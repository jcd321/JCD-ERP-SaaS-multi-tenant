namespace Jcd.Erp.Application.Common.Interfaces;

public interface IAppSettings
{
    string FrontendUrl { get; }
    int PasswordResetTokenMinutes { get; }
}
