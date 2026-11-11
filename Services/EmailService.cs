
//using MailKit.Net.Smtp;
//using MailKit.Security;
//using MimeKit;

//namespace BookStoreManagement.Services
//{
//    public class EmailService
//    {
//        private readonly IConfiguration _configuration;

//        public EmailService(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        public async Task SendPasswordResetEmailAsync(
//            string email,
//            string resetLink)
//        {
//            var message = new MimeMessage();

//            var senderName = _configuration["EmailSettings:SenderName"];
//            var senderEmail = _configuration["EmailSettings:SenderEmail"];

//            message.From.Add(new MailboxAddress(senderName, senderEmail));
//            message.To.Add(MailboxAddress.Parse(email));

//            message.Subject = "Reset Your BookStore Password";

//            message.Body = new BodyBuilder
//            {
//                HtmlBody = $@"
//                    <h2>Password Reset Request</h2>
//                    <p>We received a request to reset your password.</p>
//                    <p>Click the link below to create a new password:</p>
//                    <a href='{resetLink}'>Reset Password</a>
//                    <p>This link expires in 15 minutes.</p>
//                    <p>If you did not request this, please ignore this email.</p>"
//            }.ToMessageBody();

//            using var client = new SmtpClient();

//            var smtpServer = _configuration["EmailSettings:SmtpServer"];
//            var smtpPort = int.Parse(
//                _configuration["EmailSettings:SmtpPort"]!
//            );

//            var username = _configuration["EmailSettings:Username"];
//            var password = _configuration["EmailSettings:Password"];

//            await client.ConnectAsync(
//                smtpServer,
//                smtpPort,
//                SecureSocketOptions.StartTls
//            );

//            await client.AuthenticateAsync(username, password);

//            await client.SendAsync(message);

//            await client.DisconnectAsync(true);
//        }
//    }
//}
