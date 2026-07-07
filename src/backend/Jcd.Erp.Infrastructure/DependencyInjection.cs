using System.Text;
using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Infrastructure.Auth;
using Jcd.Erp.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Jcd.Erp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
            ?? throw new InvalidOperationException("JWT settings are not configured.");

        if (string.IsNullOrWhiteSpace(jwtSettings.Secret))
            throw new InvalidOperationException("JWT Secret is not configured.");

        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IDateTimeService, DateTimeService>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ICurrentTenantService, CurrentTenantService>();

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub,
                    RoleClaimType = System.Security.Claims.ClaimTypes.Role
                };
            });

        services.AddAuthorization();

        return services;
    }
}
