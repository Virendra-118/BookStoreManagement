
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using BookStoreManagement.DTOs;
using BookStoreManagement.Models;

using MongoDB.Driver;

namespace BookStoreManagement.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IConfiguration configuration)
        {
            var connectionString =
                configuration["MongoDB:ConnectionString"];

            var databaseName =
                configuration["MongoDB:DatabaseName"];

            var client = new MongoClient(connectionString);

            var database = client.GetDatabase(databaseName);

            _users = database.GetCollection<User>("Users");
        }

        public async Task<User?> RegisterAsync(RegisterRequest request)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            // Check whether the email already exists
            var existingUser = await _users
                .Find(user => user.Email == email)
                .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                return null;
            }

            // Hash the password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(
                request.Password
            );

            // Create user
            var user = new User
            {
                FullName = request.FullName.Trim(),
                Email = email,
                PasswordHash = passwordHash,
                Role = "Customer",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            // Save user in MongoDB
            await _users.InsertOneAsync(user);

            return user;
        }

        public async Task<User?> LoginAsync(LoginRequest request)
        {
            // Find user by email
            var email = request.Email.Trim().ToLowerInvariant();

            var user = await _users
                .Find(user => user.Email == email)
                .FirstOrDefaultAsync();

            // Check whether user exists
            if (user == null)
            {
                return null;
            }

            // Check whether user is active
            if (!user.IsActive)
            {
                return null;
            }

            // Verify password using BCrypt
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(
                request.Password,
                user.PasswordHash
            );

            if (!isPasswordValid)
            {
                return null;
            }

            return user;
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            var user = await _users
                .Find(user => user.Email == email)
                .FirstOrDefaultAsync();

            if (user == null || !user.IsActive)
            {
                return false;
            }

            // Generate a secure random token
            byte[] tokenBytes = RandomNumberGenerator.GetBytes(32);

            string resetToken = WebEncoders.Base64UrlEncode(tokenBytes);

            // Hash the token using SHA256
            byte[] tokenHashBytes = SHA256.HashData(
                Encoding.UTF8.GetBytes(resetToken)
            );

            string tokenHash = Convert.ToBase64String(tokenHashBytes);

            // Set token expiration
            user.ResetTokenHash = tokenHash;
            user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);

            // Update user in MongoDB
            await _users.ReplaceOneAsync(
                existingUser => existingUser.Id == user.Id,
                user
            );

            // Temporary development output for testing only
            Console.WriteLine($"Reset token: {resetToken}");

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
        {
            // Hash the token received from the user
            byte[] tokenHashBytes = SHA256.HashData(
                Encoding.UTF8.GetBytes(request.Token)
            );

            string tokenHash = Convert.ToBase64String(tokenHashBytes);

            // Get current UTC time
            var currentTime = DateTime.UtcNow;

            // Find user with a valid reset token
            var user = await _users
                .Find(user =>
                    user.ResetTokenHash == tokenHash &&
                    user.ResetTokenExpiry != null &&
                    user.ResetTokenExpiry > currentTime &&
                    user.IsActive)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return false;
            }

            // Hash the new password using BCrypt
            string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(
                request.NewPassword
            );

            // Update password and clear reset token
            var update = Builders<User>.Update
                .Set(user => user.PasswordHash, newPasswordHash)
                .Set(user => user.ResetTokenHash, null)
                .Set(user => user.ResetTokenExpiry, null);

            // Update MongoDB
            var result = await _users.UpdateOneAsync(
                existingUser =>
                    existingUser.Id == user.Id &&
                    existingUser.ResetTokenHash == tokenHash &&
                    existingUser.ResetTokenExpiry != null &&
                    existingUser.ResetTokenExpiry > currentTime,
                update
            );

            return result.ModifiedCount > 0;
        }


        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _users.Find(_ => true).ToListAsync();

            return users.Select(user => new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
            }).ToList();
        }

    }
}