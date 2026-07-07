using System.Net.Http.Json;
using FluentAssertions;
using Jcd.Erp.Application.Roles.Queries.GetRoles;
using Jcd.Erp.Application.Users.Queries.GetUsers;

namespace Jcd.Erp.Integration.Tests;

[Collection(nameof(IntegrationTestCollection))]
public sealed class TenantIsolationTests
{
    private readonly HttpClient _client;

    public TenantIsolationTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Users_And_Roles_Are_Isolated_Between_Tenants()
    {
        var slugA = IntegrationTestHelpers.UniqueSlug("tenant-a");
        var slugB = IntegrationTestHelpers.UniqueSlug("tenant-b");

        var tenantA = await IntegrationTestHelpers.RegisterTenantAsync(_client, slugA);
        var tenantB = await IntegrationTestHelpers.RegisterTenantAsync(_client, slugB);

        IntegrationTestHelpers.SetBearerToken(_client, tenantA.AccessToken);

        var usersA = await _client.GetFromJsonAsync<List<UserDto>>("/api/v1/users");
        var rolesA = await _client.GetFromJsonAsync<List<RoleDto>>("/api/v1/roles");

        usersA.Should().NotBeNull();
        usersA!.Should().HaveCount(1);
        usersA[0].Email.Should().Be($"admin-{slugA}@integration.test");
        usersA.Should().NotContain(u => u.Email == $"admin-{slugB}@integration.test");

        rolesA.Should().NotBeNull();
        rolesA!.Should().ContainSingle(r => r.Name == "Administrator" && r.IsSystem);

        IntegrationTestHelpers.SetBearerToken(_client, tenantB.AccessToken);

        var usersB = await _client.GetFromJsonAsync<List<UserDto>>("/api/v1/users");
        var rolesB = await _client.GetFromJsonAsync<List<RoleDto>>("/api/v1/roles");

        usersB.Should().NotBeNull();
        usersB!.Should().HaveCount(1);
        usersB[0].Email.Should().Be($"admin-{slugB}@integration.test");
        usersB.Should().NotContain(u => u.Email == $"admin-{slugA}@integration.test");

        rolesB.Should().NotBeNull();
        rolesB!.Should().ContainSingle(r => r.Name == "Administrator" && r.IsSystem);

        tenantA.TenantId.Should().NotBe(tenantB.TenantId);
    }

    [Fact]
    public async Task TenantA_Cannot_Access_TenantB_Data_With_TenantA_Token()
    {
        var slugA = IntegrationTestHelpers.UniqueSlug("iso-a");
        var slugB = IntegrationTestHelpers.UniqueSlug("iso-b");

        var tenantA = await IntegrationTestHelpers.RegisterTenantAsync(_client, slugA);
        await IntegrationTestHelpers.RegisterTenantAsync(_client, slugB);

        IntegrationTestHelpers.SetBearerToken(_client, tenantA.AccessToken);

        var users = await _client.GetFromJsonAsync<List<UserDto>>("/api/v1/users");
        users.Should().NotBeNull();
        users!.Should().OnlyContain(u => u.Email.Contains(slugA, StringComparison.OrdinalIgnoreCase));
    }
}
