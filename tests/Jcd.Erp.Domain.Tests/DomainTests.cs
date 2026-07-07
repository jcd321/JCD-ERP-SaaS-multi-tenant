using FluentAssertions;
using Jcd.Erp.Domain.Identity;
using Jcd.Erp.Domain.Tenancy;

namespace Jcd.Erp.Domain.Tests;

public class TenantTests
{
    [Fact]
    public void Create_WithEmptyName_ShouldReturnFailure()
    {
        var result = Tenant.Create("");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Tenant.NameRequired");
    }

    [Fact]
    public void Create_WithValidName_ShouldReturnSuccess()
    {
        var result = Tenant.Create("Acme Corp", "acme-corp");

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Acme Corp");
        result.Value.Slug.Should().Be("acme-corp");
        result.Value.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData("Mi Empresa", "mi-empresa")]
    [InlineData("Test 123!", "test-123")]
    public void NormalizeSlug_ShouldSanitizeInput(string input, string expected)
    {
        Tenant.NormalizeSlug(input).Should().Be(expected);
    }
}

public class UserTests
{
    [Fact]
    public void Create_WithEmptyEmail_ShouldReturnFailure()
    {
        var result = User.Create(Guid.NewGuid(), "", "hash", "John", "Doe");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("User.EmailRequired");
    }

    [Fact]
    public void Create_WithValidData_ShouldNormalizeEmail()
    {
        var tenantId = Guid.NewGuid();
        var result = User.Create(tenantId, "  Admin@Test.COM  ", "hash", "John", "Doe");

        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be("admin@test.com");
        result.Value.TenantId.Should().Be(tenantId);
    }
}
