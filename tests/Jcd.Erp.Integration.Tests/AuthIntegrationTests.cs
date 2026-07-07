using System.Net.Http.Json;
using FluentAssertions;

namespace Jcd.Erp.Integration.Tests;

[Collection(nameof(IntegrationTestCollection))]
public sealed class AuthIntegrationTests
{
    private readonly HttpClient _client;

    public AuthIntegrationTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_And_Login_Should_Succeed()
    {
        var slug = IntegrationTestHelpers.UniqueSlug("auth");
        var email = $"admin-{slug}@integration.test";

        var registered = await IntegrationTestHelpers.RegisterTenantAsync(_client, slug, email);

        registered.TenantSlug.Should().Be(slug);
        registered.AccessToken.Should().NotBeNullOrWhiteSpace();
        registered.RefreshToken.Should().NotBeNullOrWhiteSpace();

        IntegrationTestHelpers.ClearBearerToken(_client);

        var login = await IntegrationTestHelpers.LoginAsync(_client, email, slug);

        login.TenantSlug.Should().Be(slug);
        login.Email.Should().Be(email);
        login.AccessToken.Should().NotBeNullOrWhiteSpace();
        login.Permissions.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ForgotPassword_With_Unknown_Email_Returns_Success()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/forgot-password", new
        {
            email = "nobody@integration.test",
            tenantSlug = (string?)null
        });

        response.IsSuccessStatusCode.Should().BeTrue();
    }
}
