using Jcd.Erp.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Jcd.Erp.Infrastructure.Email;

public sealed class ConsoleEmailService : IEmailService
{
    private readonly ILogger<ConsoleEmailService> _logger;

    public ConsoleEmailService(ILogger<ConsoleEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendPasswordResetEmailAsync(
        string toEmail,
        string resetLink,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Password reset email for {Email}. Reset link: {ResetLink}",
            toEmail,
            resetLink);

        return Task.CompletedTask;
    }
}
