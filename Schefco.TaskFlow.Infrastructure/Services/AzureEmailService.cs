using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Options;
using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Application.Settings;

namespace Schefco.TaskFlow.Infrastructure.Services
{
    public class AzureEmailService : IEmailService
    {
        private readonly EmailClient _emailClient;

        private const string SenderAddress = "DoNotReply@f0b10ac9-90da-4068-80d3-f33bb6af5967.azurecomm.net";
        private const string SenderName = "TaskFlow Notifications";

        public AzureEmailService(IOptions<AzureCommunicationServicesSettings> acsOptions)
        {
            // Initialize the email service connection string
            var settings = acsOptions.Value;
            _emailClient = new EmailClient(settings.ConnectionString);
        }

        public Task SendPendingApprovalEmailAsync(string toEmail, string name)
        {
            // create EmailContent
            var subject = "Your TaskFlow Registration is Pending Approval";
            var body = $"" +
                $"<p>Hi, {name},</p>" +
                $"<p>Thanks for registering for TaskFlow. Your account is pending admin approval. We'll notify you once it is approved.</p>";
            var content = new EmailContent(subject);
            content.Html = body;

            // Creat the EmailMessage
            var email = new EmailMessage(SenderAddress, subject, content);
            email.Recipients.To.Add(new EmailAddress(toEmail));

            // Send the email
            return _emailClient.SendAsync(WaitUntil.Completed, email);
        }

        public Task SendTempPasswordEmailAsync(string toEmail, string name, string tempPassword)
        {
            // Create EmailContent
            var subject = "Your TaskFlow Temporary Password";
            var body = $"" +
                $"<p>Hi {name},</p>" +
                $"<p>Your temporary password is: <strong>{tempPassword}</strong></p>" +
                $"<p>Please use this password to log in to TaskFlow. You will then be prompted to create a new password.</p>" +
                $"<!-- Add login link -->";
            var content = new EmailContent(subject);
            content.Html = body;

            // Create the EmailMessage
            var email = new EmailMessage(SenderAddress, subject, content);
            email.Recipients.To.Add(new EmailAddress(toEmail));

            // send the email
            return _emailClient.SendAsync(WaitUntil.Completed, email);
        }
    }
}
