using System.Reflection;
using System.Text;
using BonusSystem.Api.Infrastructure.Swagger;
using BonusSystem.Api.Infrastructure.Swagger.Documentation;
using BonusSystem.Core.Repositories;
using BonusSystem.Core.Services.Implementations.BFF;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Infrastructure.Auth;
using BonusSystem.Infrastructure.DataAccess;
using BonusSystem.Infrastructure.DataAccess.EntityFramework;
using BonusSystem.Infrastructure.DataAccess.Seeding;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BonusSystem.Api.Infrastructure.Extensions;

public static class ApiExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure CORS - Allow everything
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        // Configure swagger and OpenAPI
        services.AddEndpointsApiExplorer();

        // Configure swagger with examples
        ConfigureSwagger(services);

        // Configure database options
        services.Configure<AppDbOptions>(
            configuration.GetSection(AppDbOptions.Position));

        // Configure Entity Framework context
        ConfigureEntityFramework(services, configuration);

        // Register authentication service
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        
        // Register DataSeeder
        services.AddScoped<DatabaseSeeder>();

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

    public static void UseApiMiddleware(this WebApplication app, IWebHostEnvironment env)
    {
        // Initialize database if using PostgreSQL
        using (var scope = app.Services.CreateScope())
        {
            try
            {
                var context = scope.ServiceProvider.GetRequiredService<BonusSystemContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                if (context.Database.GetPendingMigrations().Any())
                {
                    context.Database.Migrate();
                    logger.LogInformation("Database migrations applied successfully");
                }

                var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
                seeder.SeedAsync().Wait();

            }
            catch (Exception e)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(e, "An error occured while applying migrations");

                if (env.IsDevelopment())
                    throw;
            }
        }

        // Configure the HTTP request pipeline

        // Use CORS before other middleware
        app.UseCors();
        // Enable static files for Swagger UI customization
        app.UseStaticFiles();

        app.UseSwagger(c => { c.RouteTemplate = "api-docs/{documentName}/swagger.json"; });

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/api-docs/v1/swagger.json", "BonusSystem API v1");
            c.RoutePrefix = "api-docs";
            c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
            c.DefaultModelsExpandDepth(0); // Hide models section by default
            c.DisplayRequestDuration();
            c.EnableDeepLinking();
            c.EnableFilter();
            c.EnableValidator();
            c.DocumentTitle = "BonusSystem API Documentation";
        });

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        // Add a root endpoint that redirects to API docs
        app.MapGet("/", () => Results.Redirect("/api-docs"))
            .ExcludeFromDescription();
    }

    private static void ConfigureSwagger(IServiceCollection services)
    {
        // Configure Swagger with examples
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "BonusSystem API",
                Version = "v1",
                Description =
                    "API for the BonusSystem - a bonus points loyalty program management platform with the following features:\n\n" +
                    "- User management with different roles (Buyers, Sellers, Admins, Observers)\n" +
                    "- Company and store registration and management\n" +
                    "- Bonus points transactions (earning, spending, expiring)\n" +
                    "- QR code generation for buyer identification\n" +
                    "- Analytics and reporting",
                Contact = new OpenApiContact
                {
                    Name = "BonusSystem Team",
                    Email = "info@bonussystem.com"
                }
            });

            // Add JWT authentication to Swagger
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description =
                    "JWT Authorization header using the Bearer scheme. Enter 'Bearer {token}' in the field below.",
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

            // Organize endpoints by tag
            c.TagActionsBy(api => new[] { api.GroupName ?? "Default" });
            c.DocInclusionPredicate((name, api) => true);

            // Add schema enhancement filter
            c.DocumentFilter<SchemaEnhancementFilter>();

            // Add response example providers
            c.OperationFilter<AuthExamples>();
            c.OperationFilter<TransactionExamples>();
            c.OperationFilter<CompanyStoreExamples>();
            c.OperationFilter<AdminExamples>();
            c.OperationFilter<ObserverExamples>();
            c.OperationFilter<BuyerSpecialtyExamples>();

            // Customize operation IDs
            c.CustomOperationIds(e => e.ActionDescriptor.EndpointMetadata
                .OfType<RouteNameMetadata>()
                .FirstOrDefault()?.RouteName);

            // Include XML comments if available
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }

            // Configure examples for response content types
            c.ExampleFilters();
        });

        // Register Swagger example providers 
        services.AddSwaggerExamplesFromAssemblyOf<Program>();

        // Configure Swagger with custom options
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigurationOptions>();
    }

    private static void ConfigureEntityFramework(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Connection string not found in the configuration");
        
        // Configure the db context
        services.AddDbContext<BonusSystemContext>(options =>
        {
            options.UseNpgsql(connectionString, npgOptions =>
            {
                npgOptions.MigrationsAssembly(typeof(BonusSystemContext).Assembly.GetName().Name);
                npgOptions.EnableRetryOnFailure(5);
            });
            
            // Enable detailed logging in development
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        services.AddScoped<IDataService, EntityFrameworkDataService>();

        // Register repositories directly for cases when they're injected directly
        services.AddScoped<IUserRepository>(sp => sp.GetRequiredService<IDataService>().Users);
        services.AddScoped<ICompanyRepository>(sp => sp.GetRequiredService<IDataService>().Companies);
        services.AddScoped<IStoreRepository>(sp => sp.GetRequiredService<IDataService>().Stores);
        services.AddScoped<ITransactionRepository>(sp => sp.GetRequiredService<IDataService>().Transactions);
        services.AddScoped<INotificationRepository>(sp => sp.GetRequiredService<IDataService>().Notifications);
    }
    
}