using Jcd.Erp.Domain.Common;

namespace Jcd.Erp.Domain.Identity;

public class User : BaseAuditableEntity
{
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public bool EmailConfirmed { get; set; }

    public ICollection<UserRole> UserRoles { get; private set; } = [];

    private User() { }

    public static Result<User> Create(
        Guid tenantId,
        string email,
        string passwordHash,
        string firstName,
        string lastName)
    {
        if (tenantId == Guid.Empty)
            return Result.Failure<User>("User.TenantRequired");

        if (string.IsNullOrWhiteSpace(email))
            return Result.Failure<User>("User.EmailRequired");

        if (string.IsNullOrWhiteSpace(passwordHash))
            return Result.Failure<User>("User.PasswordRequired");

        if (string.IsNullOrWhiteSpace(firstName))
            return Result.Failure<User>("User.FirstNameRequired");

        return Result.Success(new User
        {
            TenantId = tenantId,
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            FirstName = firstName.Trim(),
            LastName = lastName?.Trim() ?? string.Empty,
            IsActive = true,
            EmailConfirmed = false,
            CreatedAt = DateTime.UtcNow
        });
    }

    public string FullName => $"{FirstName} {LastName}".Trim();

    public void UpdateProfile(string firstName, string lastName)
    {
        FirstName = firstName.Trim();
        LastName = lastName?.Trim() ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
