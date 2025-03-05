using System.Text;
using BonusSystem.Core.Repositories;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Infrastructure.Auth;
using BonusSystem.Infrastructure.DataAccess;
using BonusSystem.Infrastructure.DataAccess.InMemory;
using BonusSystem.Infrastructure.DataAccess.Postgres;
using BonusSystem.Shared.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

public class Program
{
    public static async Task Main(string[] args)
    {
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
            
        // Configure PostgreSQL options
        builder.Services.Configure<PostgresOptions>(
            builder.Configuration.GetSection(PostgresOptions.Position));

        // Register data services based on configuration
        var dbType = builder.Configuration["AppDb:Type"];

        if (dbType?.ToLower() == "postgres")
        {
            // Configure PostgreSQL DbContext
            builder.Services.AddDbContext<BonusSystemDbContext>((serviceProvider, options) =>
            {
                var postgresOptions = serviceProvider.GetRequiredService<IOptions<PostgresOptions>>().Value;
                
                options.UseNpgsql(postgresOptions.ConnectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsHistoryTable("__ef_migrations_history", "bonus");
                    npgsqlOptions.EnableRetryOnFailure(5);
                });
                
                if (postgresOptions.EnableDetailedLogging)
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }
            });
            
            // Register PostgreSQL data service
            builder.Services.AddScoped<IDataService, PostgresDataService>();
            
            // Register PostgreSQL initializer
            builder.Services.AddScoped<PostgresInitializer>();
        }
        else
        {
            // Register InMemory data service (fallback)
            builder.Services.AddSingleton<IDataService, InMemoryDataService>();
        }
        
        // Register repositories based on data service (will resolve according to configuration)
        builder.Services.AddScoped<IUserRepository>(sp => sp.GetRequiredService<IDataService>().Users);
        builder.Services.AddScoped<ICompanyRepository>(sp => sp.GetRequiredService<IDataService>().Companies);
        builder.Services.AddScoped<IStoreRepository>(sp => sp.GetRequiredService<IDataService>().Stores);
        builder.Services.AddScoped<ITransactionRepository>(sp => sp.GetRequiredService<IDataService>().Transactions);
        builder.Services.AddScoped<INotificationRepository>(sp => sp.GetRequiredService<IDataService>().Notifications);

        // Register authentication service
        builder.Services.AddScoped<IAuthenticationService, JwtAuthenticationService>();

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

        // Initialize database if using PostgreSQL
        if (dbType?.ToLower() == "postgres")
        {
            using (var scope = app.Services.CreateScope())
            {
                var initializer = scope.ServiceProvider.GetRequiredService<PostgresInitializer>();
                await initializer.InitializeAsync();
            }
        }

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

        await app.RunAsync();
    }    
}