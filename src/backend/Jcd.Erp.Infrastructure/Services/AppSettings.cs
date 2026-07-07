using Jcd.Erp.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Jcd.Erp.Infrastructure.Services;

public sealed class AppSettings : IAppSettings
{
    public AppSettings(IConfiguration configuration)
    {
        FrontendUrl = configuration["App:FrontendUrl"] ?? "http://localhost:4200";
        PasswordResetTokenMinutes = configuration.GetValue("App:PasswordResetTokenMinutes", 60);
    }

    public string FrontendUrl { get; }
    public int PasswordResetTokenMinutes { get; }
}
