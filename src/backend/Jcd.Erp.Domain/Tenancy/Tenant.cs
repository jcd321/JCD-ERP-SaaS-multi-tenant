using Jcd.Erp.Domain.Common;
using System.Text.RegularExpressions;

namespace Jcd.Erp.Domain.Tenancy;

public class Tenant : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Tenant() { }

    public static Result<Tenant> Create(string name, string? slug = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Tenant>("Tenant.NameRequired");

        var normalizedSlug = NormalizeSlug(slug ?? name);
        if (string.IsNullOrEmpty(normalizedSlug))
            return Result.Failure<Tenant>("Tenant.SlugInvalid");

        return Result.Success(new Tenant
        {
            Name = name.Trim(),
            Slug = normalizedSlug,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });
    }

    public void Deactivate() => IsActive = false;

    public static string NormalizeSlug(string value)
    {
        var slug = value.Trim().ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", string.Empty);
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-").Trim('-');
        return slug.Length > 100 ? slug[..100].Trim('-') : slug;
    }
}
