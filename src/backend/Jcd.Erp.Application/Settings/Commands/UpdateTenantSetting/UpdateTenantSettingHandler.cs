using Jcd.Erp.Application.Common.Cache;
using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Configuration;
using MediatR;

namespace Jcd.Erp.Application.Settings.Commands.UpdateTenantSetting;

public class UpdateTenantSettingHandler : IRequestHandler<UpdateTenantSettingCommand, Result>
{
    private readonly ITenantSettingRepository _repository;
    private readonly ICurrentTenantService _tenant;
    private readonly ICacheService _cache;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTenantSettingHandler(
        ITenantSettingRepository repository,
        ICurrentTenantService tenant,
        ICacheService cache,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _tenant = tenant;
        _cache = cache;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateTenantSettingCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure("Auth.TenantRequired");

        var key = request.Key.Trim().ToLowerInvariant();
        var existing = await _repository.GetByKeyAsync(key, cancellationToken);

        if (existing is null)
            return Result.Failure("Settings.NotFound");

        existing.UpdateValue(request.Value);
        _repository.Update(existing);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cache.RemoveByPrefixAsync(CacheKeys.TenantSettingsPrefix(_tenant.TenantId), cancellationToken);

        return Result.Success();
    }
}
