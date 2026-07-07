namespace Jcd.Erp.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(
        string toEmail,
        string resetLink,
        CancellationToken cancellationToken = default);
}
