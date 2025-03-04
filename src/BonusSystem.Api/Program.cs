using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Infrastructure.Supabase;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Supabase
builder.Services.Configure<SupabaseOptions>(
    builder.Configuration.GetSection(SupabaseOptions.Position));

// Configure Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSecret = builder.Configuration["Supabase:JwtSecret"];
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

// Register BFF services
// These would be implemented and registered when ready
// builder.Services.AddScoped<IBuyerBffService, BuyerBffService>();
// builder.Services.AddScoped<ISellerBffService, SellerBffService>();
// builder.Services.AddScoped<IAdminBffService, AdminBffService>();
// builder.Services.AddScoped<IObserverBffService, ObserverBffService>();

// Register infrastructure services
// builder.Services.AddScoped<ISupabaseService, SupabaseService>();
// builder.Services.AddScoped<IAuthenticationService, SupabaseAuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Authentication endpoints
app.MapPost("/auth/register", async (HttpContext httpContext) =>
{
    // Implementation would go here
    return Results.Ok(new { message = "Registration endpoint placeholder" });
})
.AllowAnonymous()
.WithName("Register")
.WithOpenApi();

app.MapPost("/auth/login", async (HttpContext httpContext) =>
{
    // Implementation would go here
    return Results.Ok(new { message = "Login endpoint placeholder" });
})
.AllowAnonymous()
.WithName("Login")
.WithOpenApi();

// Role-specific endpoints would be added here
// Each role would have its own group of endpoints

app.Run();