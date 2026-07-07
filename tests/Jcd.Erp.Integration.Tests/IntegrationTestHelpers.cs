using System.Net.Http.Headers;
using System.Net.Http.Json;
using Jcd.Erp.Application.Auth.Commands.Login;
using Jcd.Erp.Application.Auth.Commands.RegisterTenant;

namespace Jcd.Erp.Integration.Tests;

internal static class IntegrationTestHelpers
{
    public static async Task<RegisterTenantResponse> RegisterTenantAsync(
        HttpClient client,
        string slug,
        string? adminEmail = null)
    {
        var email = adminEmail ?? $"admin-{slug}@integration.test";

        var response = await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            companyName = $"Company {slug}",
            slug,
            adminEmail = email,
            adminPassword = "Password123!",
            adminFirstName = "Admin",
            adminLastName = slug
        });

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<RegisterTenantResponse>();
        return body ?? throw new InvalidOperationException("Register response was empty.");
    }

    public static async Task<LoginResponse> LoginAsync(
        HttpClient client,
        string email,
        string tenantSlug,
        string password = "Password123!")
    {
        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email,
            password,
            tenantSlug,
            rememberMe = false
        });

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return body ?? throw new InvalidOperationException("Login response was empty.");
    }

    public static void SetBearerToken(HttpClient client, string accessToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }

    public static void ClearBearerToken(HttpClient client) =>
        client.DefaultRequestHeaders.Authorization = null;

    public static string UniqueSlug(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}"[..24];
}
