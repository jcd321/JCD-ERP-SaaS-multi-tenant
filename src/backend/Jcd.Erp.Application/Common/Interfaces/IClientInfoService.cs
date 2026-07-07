namespace Jcd.Erp.Application.Common.Interfaces;

public interface IClientInfoService
{
    string? IpAddress { get; }

    string? UserAgent { get; }
}
