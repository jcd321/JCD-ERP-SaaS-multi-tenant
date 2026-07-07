using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Settings.Queries.GetTenantSettings;

public record GetTenantSettingsQuery : IRequest<Result<IReadOnlyList<TenantSettingDto>>>;

public record TenantSettingDto(string Key, string Value);
