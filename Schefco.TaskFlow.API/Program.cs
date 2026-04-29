using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.API.Endpoints;
using Schefco.TaskFlow.API.Extensions;
using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Application.Features.Auth;
using Schefco.TaskFlow.Infrastructure.Persistence.Repositories;
using Schefco.TaskFlow.Infrastructure.Persistence;
using Schefco.TaskFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Schefco.TaskFlow.Application.Common.Settings;
using Microsoft.Extensions.Options;
using Schefco.TaskFlow.Infrastructure.Services;
using Schefco.TaskFlow.Infrastructure.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Routing.Matching;
using FluentValidation;
using System.IdentityModel.Tokens.Jwt;
using Schefco.TaskFlow.Application.Settings;
using Schefco.TaskFlow.API.MIddleware;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("HTTP_PLATFORM_PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// CORS service
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "https://schefco.com").AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed(origin => true);
        });
});

// Register Mediator engine
builder.Services.AddScoped<IMediator, Mediator>();

// Get the Application assembly
var applicationAssembly = typeof(LoginCommand).Assembly;

// Register all ICommandHandlers
foreach (var type in applicationAssembly.GetTypes())
{
    var interfaces = type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>));

    foreach (var handlerInterface in interfaces)
    {
        builder.Services.AddScoped(handlerInterface, type);
    }
}

// Register all IQueryHandlers
foreach (var type in applicationAssembly.GetTypes())
{
    var interfaces = type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));

    foreach (var handlerInterface in interfaces)
    {
        builder.Services.AddScoped(handlerInterface, type);
    }
}

// builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApiServices();

// Program DBs
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(Environment.GetEnvironmentVariable("DATABASE_URL")));
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPendingUserRepository, PendingUserRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

// Register HTTP Context Accessor
builder.Services.AddHttpContextAccessor();

// Provides current token read service
builder.Services.AddScoped<ICurrentTokenService, CurrentTokenService>();

// Provides ASP.NET Identity's password hashing implementation
builder.Services.AddScoped<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();

// Wraps the Identity hasher so Application layer can use service but stay clean to our Architecture
builder.Services.AddScoped<IPasswordHashingService, PasswordHashingService>();

// Provides JWT generation service
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// Provides email service
builder.Services.AddHttpClient<IEmailService, EmailService>();

// Binds Owner email from configuration
builder.Services.Configure<OwnerSettings>(builder.Configuration.GetSection("OwnerSettings"));
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<OwnerSettings>>().Value);

// Bind the AzureCommunicationServices from appsetting.json
builder.Services.Configure<AzureCommunicationServicesSettings>(builder.Configuration.GetSection("AzureCommunicationServices"));

// Keep JWT claim names consistent
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(options => 
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
})
.AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        NameClaimType = "sub",
        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorizationBuilder().AddPolicy("Owner", policy => policy.RequireRole("Owner"));

var app = builder.Build();

var contentRoot = app.Environment.ContentRootPath;

app.UseRouting();

app.UseCors("AllowFrontend");

app.UseMiddleware<TokenMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapHealthEndpoints();

app.MapProjectEndpoints();

app.MapAuthEndpoints();

app.MapTaskEndpoints();

app.MapGet("/", () => Results.Ok("TaskFlow API is running."));

// Serve file path to endpoints for scalar hook
app.MapGet("openapi.json", async context =>
{
    var filePath = Path.Combine(AppContext.BaseDirectory, "openapi.json");
    var json = await File.ReadAllTextAsync(filePath);
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsync(json);
});

// Hook Scalar into the pipeline and tell it where the OpenAPI file lives
app.MapScalarApiReference(options =>
{ 
    options.WithTitle("TaskFlow API")
    .ForceDarkMode()
    .WithOpenApiRoutePattern("/openapi.json");
});

app.Run();