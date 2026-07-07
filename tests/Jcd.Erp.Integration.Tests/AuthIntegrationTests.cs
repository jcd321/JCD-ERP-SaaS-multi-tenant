using System.Net.Http.Json;
using FluentAssertions;
using Jcd.Erp.Domain.Audit;
using Jcd.Erp.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Jcd.Erp.Integration.Tests;

[Collection(nameof(IntegrationTestCollection))]
public sealed class AuthIntegrationTests
{
    private readonly IntegrationTestWebAppFactory _factory;
    private readonly HttpClient _client;

    public AuthIntegrationTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
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

    [Fact]
    public async Task Login_And_Logout_Should_Write_Audit_Logs_And_Manage_Sessions()
    {
        var slug = IntegrationTestHelpers.UniqueSlug("audit");
        var email = $"admin-{slug}@integration.test";

        await IntegrationTestHelpers.RegisterTenantAsync(_client, slug, email);
        IntegrationTestHelpers.ClearBearerToken(_client);

        var login = await IntegrationTestHelpers.LoginAsync(_client, email, slug);

        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.CurrentTenantId = login.TenantId;

            var loginAudits = await db.AuditLogs
                .Where(log => log.UserId == login.UserId && log.Action == AuditAction.Login)
                .ToListAsync();

            loginAudits.Should().HaveCount(1);

            var activeSessions = await db.UserSessions
                .Where(session => session.UserId == login.UserId && !session.IsRevoked)
                .ToListAsync();

            activeSessions.Should().HaveCount(1);
        }

        IntegrationTestHelpers.SetBearerToken(_client, login.AccessToken);

        var logoutResponse = await _client.PostAsJsonAsync("/api/v1/auth/logout", new
        {
            refreshToken = login.RefreshToken
        });

        logoutResponse.IsSuccessStatusCode.Should().BeTrue();

        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.CurrentTenantId = login.TenantId;

            var logoutAudits = await db.AuditLogs
                .Where(log => log.UserId == login.UserId && log.Action == AuditAction.Logout)
                .ToListAsync();

            logoutAudits.Should().HaveCount(1);

            var activeSessions = await db.UserSessions
                .Where(session => session.UserId == login.UserId && !session.IsRevoked)
                .ToListAsync();

            activeSessions.Should().BeEmpty();
        }
    }
}
