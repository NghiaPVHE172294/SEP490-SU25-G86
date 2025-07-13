using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.EmailDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.RemindRepository;
using System.Net;
using System.Net.Mail;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.RemindService
{
    public class RemindService : IRemindService
    {
        private readonly IConfiguration _configuration;
        private readonly IRemindRepository _remindRepository;
        public RemindService(IConfiguration configuration, IRemindRepository remindRepository)
        {
            _configuration = configuration;
            _remindRepository = remindRepository;
        }
        public async Task SendReminderAsync(ReminderEmailRequestDTO request)
        {
            string smtpServer = _configuration["EmailSettings:SmtpServer"];
            int smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            string smtpUser = _configuration["EmailSettings:SmtpUser"];
            string smtpPass = _configuration["EmailSettings:SmtpPass"];
            string senderEmail = _configuration["EmailSettings:SenderEmail"];
            string senderName = _configuration["EmailSettings:SenderName"];

            var smtpClient = new SmtpClient(smtpServer)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = request.Subject,
                Body = request.Message,
                IsBodyHtml = true
            };

            mailMessage.To.Add(request.ToEmail);

            await smtpClient.SendMailAsync(mailMessage);

            // Lưu lịch sử
            var remind = new Remind
            {
                FromEmail = senderEmail,
                ToEmail = request.ToEmail,
                Title = request.Subject,
                Message = request.Message,
                CreateAt = DateTime.UtcNow
            };

            await _remindRepository.SaveRemindAsync(remind);
        }
    }
}
