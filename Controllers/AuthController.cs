
using Microsoft.AspNetCore.Authorization;
using BookStoreManagement.DTOs;
using BookStoreManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly TokenService _tokenService;

        public AuthController(UserService userService, TokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var user = await _userService.RegisterAsync(request);

            if (user == null)
            {
                return Conflict("Email already registered.");
            }

            return Created("", new
            {
                message = "User registered successfully.",
                user.Id,
                user.FullName,
                user.Email,
                user.Role,
                user.CreatedAt
            });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _userService.LoginAsync(request);

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            var tokenResult = _tokenService.GenerateToken(user);

            return Ok(new
            {
                message = "Login successful",
                token = tokenResult.Token,
                expiresAt = tokenResult.ExpiresAt,
                user = new
                {
                    id = user.Id,
                    fullName = user.FullName,
                    email = user.Email,
                    
                    role = user.Role
                }
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
    ForgotPasswordRequest request)
        {
            await _userService.ForgotPasswordAsync(request);

            return Ok(new
            {
                message = "If the account exists, a password reset link will be sent."
            });
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
    ResetPasswordRequest request)
        {
            var result = await _userService.ResetPasswordAsync(request);

            if (!result)
            {
                return BadRequest(new
                {
                    message = "Invalid or expired reset token."
                });
            }

            return Ok(new
            {
                message = "Password reset successfully."
            });
        }

    }
}
