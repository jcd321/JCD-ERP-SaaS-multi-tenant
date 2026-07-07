using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Settings.Commands.UpdateTenantSetting;

public record UpdateTenantSettingCommand(string Key, string Value) : IRequest<Result>;
