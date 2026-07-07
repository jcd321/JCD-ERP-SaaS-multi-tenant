using System.Threading.RateLimiting;
using Jcd.Erp.Api.Extensions;
using Jcd.Erp.Api.Middleware;
using Jcd.Erp.Application;
using Jcd.Erp.Infrastructure;
using Jcd.Erp.Persistence;
using Jcd.Erp.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console());

builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPermissionPolicies();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? ["http://localhost:4200"])
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString ?? "Host=localhost;Database=jcd_erp;Username=postgres;Password=postgres")
    .AddRedis(builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379");

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseRateLimiter();
app.UseAuthentication();
app.UseMiddleware<TenantResolutionMiddleware>();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Jcd.Erp.Persistence.Context.ApplicationDbContext>();
    await db.Database.MigrateAsync();
    await scope.ServiceProvider.GetRequiredService<DatabaseSeeder>().SeedAsync();
}

app.Run();

public partial class Program;
