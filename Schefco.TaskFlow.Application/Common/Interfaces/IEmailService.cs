using Schefco.TaskFlow.Application.Common.Mediator;

namespace Schefco.TaskFlow.Application.Common.Interfaces
{
    // Defines the contract for sending system emails
    public interface IEmailService
    {
        // Sends an email letting the user know their reg was recieved and is pending approval
        Task SendPendingApprovalEmailAsync(string toEmail, string name);

        // Sends the temp password to the user after the admin approves registration
        Task SendTempPasswordEmailAsync(string toEmail, string name, string tempPassword);

    }
}
