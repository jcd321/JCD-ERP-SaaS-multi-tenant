using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Configuration;
using MediatR;

namespace Jcd.Erp.Application.Settings.Commands.UpdateTenantSetting;

public class UpdateTenantSettingHandler : IRequestHandler<UpdateTenantSettingCommand, Result>
{
    private readonly ITenantSettingRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTenantSettingHandler(ITenantSettingRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateTenantSettingCommand request, CancellationToken cancellationToken)
    {
        var key = request.Key.Trim().ToLowerInvariant();
        var existing = await _repository.GetByKeyAsync(key, cancellationToken);

        if (existing is null)
            return Result.Failure("Settings.NotFound");

        existing.UpdateValue(request.Value);
        _repository.Update(existing);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
