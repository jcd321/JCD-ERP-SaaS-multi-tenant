using Jcd.Erp.Application.Common.Cache;
using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Configuration;
using MediatR;

namespace Jcd.Erp.Application.Settings.Queries.GetTenantSettings;

public class GetTenantSettingsHandler : IRequestHandler<GetTenantSettingsQuery, Result<IReadOnlyList<TenantSettingDto>>>
{
    private static readonly TimeSpan SettingsTtl = TimeSpan.FromHours(1);

    private readonly ITenantSettingRepository _repository;
    private readonly ICurrentTenantService _tenant;
    private readonly ICacheService _cache;

    public GetTenantSettingsHandler(
        ITenantSettingRepository repository,
        ICurrentTenantService tenant,
        ICacheService cache)
    {
        _repository = repository;
        _tenant = tenant;
        _cache = cache;
    }

    public async Task<Result<IReadOnlyList<TenantSettingDto>>> Handle(
        GetTenantSettingsQuery request,
        CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<IReadOnlyList<TenantSettingDto>>("Auth.TenantRequired");

        var cacheKey = CacheKeys.TenantSettings(_tenant.TenantId);
        var cached = await _cache.GetAsync<List<TenantSettingDto>>(cacheKey, cancellationToken);
        if (cached is not null)
            return Result.Success<IReadOnlyList<TenantSettingDto>>(cached);

        var settings = await _repository.GetAllAsync(cancellationToken);
        var dtos = settings.Select(s => new TenantSettingDto(s.Key, s.Value)).ToList();
        await _cache.SetAsync(cacheKey, dtos, SettingsTtl, cancellationToken);
        return Result.Success<IReadOnlyList<TenantSettingDto>>(dtos);
    }
}
