// Anwar
using BonusSystem.Core.Repositories;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Infrastructure.Auth;
using BonusSystem.Infrastructure.DataAccess;
using BonusSystem.Infrastructure.DataAccess.InMemory;
using BonusSystem.Shared.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BonusSystem API",
        Version = "v1",
        Description = "A prototype API for the BonusSystem"
    });
    
    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure database options
builder.Services.Configure<AppDbOptions>(
    builder.Configuration.GetSection(AppDbOptions.Position));

// Register repositories and data services
builder.Services.AddSingleton<IDataService, InMemoryDataService>();
builder.Services.AddSingleton<IUserRepository>(sp => sp.GetRequiredService<IDataService>().Users);
builder.Services.AddSingleton<ICompanyRepository>(sp => sp.GetRequiredService<IDataService>().Companies);
builder.Services.AddSingleton<IStoreRepository>(sp => sp.GetRequiredService<IDataService>().Stores);
builder.Services.AddSingleton<ITransactionRepository>(sp => sp.GetRequiredService<IDataService>().Transactions);
builder.Services.AddSingleton<INotificationRepository>(sp => sp.GetRequiredService<IDataService>().Notifications);

// Register authentication service
builder.Services.AddSingleton<IAuthenticationService, JwtAuthenticationService>();

// Configure Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSecret = builder.Configuration["AppDb:JwtSecret"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret ?? string.Empty)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BonusSystem API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Authentication endpoints
app.MapPost("/auth/register", async (IAuthenticationService authService, UserRegistrationDto registration) =>
{
    var result = await authService.SignUpAsync(registration);
    
    if (!result.Success)
    {
        return Results.BadRequest(new { message = result.ErrorMessage });
    }
    
    return Results.Ok(new { 
        userId = result.UserId,
        token = result.Token,
        role = result.Role
    });
})
.AllowAnonymous()
.WithName("Register")
.WithOpenApi();

app.MapPost("/auth/login", async (IAuthenticationService authService, UserLoginDto login) =>
{
    var result = await authService.SignInAsync(login);
    
    if (!result.Success)
    {
        return Results.BadRequest(new { message = result.ErrorMessage });
    }
    
    return Results.Ok(new { 
        userId = result.UserId,
        token = result.Token,
        role = result.Role
    });
})
.AllowAnonymous()
.WithName("Login")
.WithOpenApi();

// Add a root endpoint that redirects to Swagger
app.MapGet("/", () => Results.Redirect("/swagger"))
    .ExcludeFromDescription();

// Role-specific endpoints would be added here
// Each role would have its own group of endpoints

app.Run();