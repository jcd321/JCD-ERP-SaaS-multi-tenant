using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Configuration;
using MediatR;

namespace Jcd.Erp.Application.Settings.Queries.GetTenantSettings;

public class GetTenantSettingsHandler : IRequestHandler<GetTenantSettingsQuery, Result<IReadOnlyList<TenantSettingDto>>>
{
    private readonly ITenantSettingRepository _repository;

    public GetTenantSettingsHandler(ITenantSettingRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<TenantSettingDto>>> Handle(
        GetTenantSettingsQuery request,
        CancellationToken cancellationToken)
    {
        var settings = await _repository.GetAllAsync(cancellationToken);
        var dtos = settings.Select(s => new TenantSettingDto(s.Key, s.Value)).ToList();
        return Result.Success<IReadOnlyList<TenantSettingDto>>(dtos);
    }
}
