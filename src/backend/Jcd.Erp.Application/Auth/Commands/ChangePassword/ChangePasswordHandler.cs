using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Identity;
using MediatR;

namespace Jcd.Erp.Application.Auth.Commands.ChangePassword;

public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangePasswordHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUser,
        IPasswordHasher passwordHasher,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _currentUser = currentUser;
        _passwordHasher = passwordHasher;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return Result.Failure("Auth.Unauthorized");

        var user = await _userRepository.GetByIdAsync(_currentUser.UserId.Value, cancellationToken);
        if (user is null)
            return Result.Failure("User.NotFound");

        if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            return Result.Failure("Auth.InvalidCurrentPassword");

        user.ChangePassword(_passwordHasher.Hash(request.NewPassword));
        _userRepository.Update(user);

        await _refreshTokenRepository.RevokeAllForUserAsync(user.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
