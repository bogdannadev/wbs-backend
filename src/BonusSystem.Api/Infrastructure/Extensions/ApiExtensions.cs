using System.Text;
using BonusSystem.Core.Repositories;
using BonusSystem.Core.Services.Implementations.BFF;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Infrastructure.Auth;
using BonusSystem.Infrastructure.DataAccess;
using BonusSystem.Infrastructure.DataAccess.InMemory;
using BonusSystem.Infrastructure.DataAccess.Postgres;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace BonusSystem.Api.Infrastructure.Extensions;

public static class ApiExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
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
        services.Configure<AppDbOptions>(
            configuration.GetSection(AppDbOptions.Position));
            
        // Configure PostgreSQL options
        services.Configure<PostgresOptions>(
            configuration.GetSection(PostgresOptions.Position));

        // Register data services based on configuration
        var dbType = configuration["AppDb:Type"];

        if (dbType?.ToLower() == "postgres")
        {
            // Configure PostgreSQL DbContext
            services.AddDbContext<BonusSystemDbContext>((serviceProvider, options) =>
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
            services.AddScoped<IDataService, PostgresDataService>();
            
            // Register PostgreSQL initializer
            services.AddScoped<PostgresInitializer>();
        }
        else
        {
            // Register InMemory data service (fallback)
            services.AddSingleton<IDataService, InMemoryDataService>();
        }
        
        // Register repositories based on data service
        services.AddScoped<IUserRepository>(sp => sp.GetRequiredService<IDataService>().Users);
        services.AddScoped<ICompanyRepository>(sp => sp.GetRequiredService<IDataService>().Companies);
        services.AddScoped<IStoreRepository>(sp => sp.GetRequiredService<IDataService>().Stores);
        services.AddScoped<ITransactionRepository>(sp => sp.GetRequiredService<IDataService>().Transactions);
        services.AddScoped<INotificationRepository>(sp => sp.GetRequiredService<IDataService>().Notifications);

        // Register authentication service
        services.AddScoped<IAuthenticationService, JwtAuthenticationService>();

        // Register BFF services
        services.AddScoped<IBuyerBffService, BuyerBffService>();
        services.AddScoped<ISellerBffService, SellerBffService>();
        services.AddScoped<IAdminBffService, AdminBffService>();
        services.AddScoped<IObserverBffService, ObserverBffService>();

        // Configure Authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtSecret = configuration["AppDb:JwtSecret"];
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

        services.AddAuthorization();

        return services;
    }

    public static WebApplication UseApiMiddleware(this WebApplication app, IWebHostEnvironment env)
    {
        // Initialize database if using PostgreSQL
        var dbType = app.Configuration["AppDb:Type"];
        if (dbType?.ToLower() == "postgres")
        {
            using (var scope = app.Services.CreateScope())
            {
                var initializer = scope.ServiceProvider.GetRequiredService<PostgresInitializer>();
                initializer.InitializeAsync().GetAwaiter().GetResult();
            }
        }

        // Configure the HTTP request pipeline
        if (env.IsDevelopment())
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

        // Add a root endpoint that redirects to Swagger
        app.MapGet("/", () => Results.Redirect("/swagger"))
            .ExcludeFromDescription();

        return app;
    }
}
