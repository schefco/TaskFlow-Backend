using Schefco.TaskFlow.Application.Common.Interfaces;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace Schefco.TaskFlow.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        // Email service for using render
        private readonly HttpClient _http;
        private readonly string _apiKey;

        public EmailService(HttpClient http)
        {
            _http = http;
            _apiKey = Environment.GetEnvironmentVariable("RESEND_API_KEY") ?? throw new Exception("RESEND_API_KEY not set");
        }

        public async Task SendPendingApprovalEmailAsync(string toEmail, string name)
        {
            // Confirmation email after registering
            var subject = "Your TaskFlow Registration is Pending Approval";

            var html = $@"<p>Hi {name},</p> <p>Thanks for registering for TaskFlow by Schefco.</p><p>Your account is pending approval. We will notify you once it is approved.</p>";

            await SendEmailAsync(toEmail, subject, html);
        }

        public async Task SendTempPasswordEmailAsync(string toEmail, string name, string tempPassword)
        {
            // Approval email with temp password
            var subject = "Your TaskFlow Temporary Password";

            var html = $@"<p>Hi {name},</p> <p>Your temporary password is: <strong>{tempPassword}</strong></p><p>Please log in and set your new password.</p>";

            await SendEmailAsync(toEmail, subject, html);
        }

        private async Task SendEmailAsync(string to, string subject, string html)
        {
            // Async function that sends email

            // Build the email
            var payload = new
            {
                from = "TaskFlow <onboarding@schefco.com",
                to = new[] { to },
                subject,
                html
            };

            // Add in Resend API key to use service
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            // send it over to resend to send the user the email
            var response = await _http.PostAsJsonAsync("https://api.resend.com/emails", payload);

            // If it fails log the error
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new Exception($"Email send failed: {response.StatusCode} - {body}");
            }
        }
    }
}
