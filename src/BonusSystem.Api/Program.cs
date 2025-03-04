using BonusSystem.Core.Repositories;
using BonusSystem.Core.Services.BffImpl;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Infrastructure.Auth;
using BonusSystem.Infrastructure.DataAccess;
using BonusSystem.Infrastructure.DataAccess.InMemory;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;
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

// Register BFF services
builder.Services.AddScoped<IBuyerBffService, BuyerBffService>();
builder.Services.AddScoped<ISellerBffService, SellerBffService>();
builder.Services.AddScoped<IAdminBffService, AdminBffService>();
builder.Services.AddScoped<IObserverBffService, ObserverBffService>();

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

// Add a root endpoint that redirects to Swagger
app.MapGet("/", () => Results.Redirect("/swagger"))
    .ExcludeFromDescription();

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

// Role-specific endpoints
// Buyer endpoints
var buyerGroup = app.MapGroup("/api/buyer").RequireAuthorization();

buyerGroup.MapGet("/bonus-summary", async (HttpContext context, IBuyerBffService buyerService) =>
{
    var userId = GetUserIdFromContext(context);
    if (!userId.HasValue)
        return Results.Unauthorized();
        
    var summary = await buyerService.GetBonusSummaryAsync(userId.Value);
    return Results.Ok(summary);
})
.WithName("GetBuyerBonusSummary")
.WithOpenApi();

buyerGroup.MapGet("/transactions", async (HttpContext context, IBuyerBffService buyerService) =>
{
    var userId = GetUserIdFromContext(context);
    if (!userId.HasValue)
        return Results.Unauthorized();
        
    var transactions = await buyerService.GetTransactionHistoryAsync(userId.Value);
    return Results.Ok(transactions);
})
.WithName("GetBuyerTransactions")
.WithOpenApi();

buyerGroup.MapGet("/qr-code", async (HttpContext context, IBuyerBffService buyerService) =>
{
    var userId = GetUserIdFromContext(context);
    if (!userId.HasValue)
        return Results.Unauthorized();
        
    var qrCode = await buyerService.GenerateQrCodeAsync(userId.Value);
    return Results.Ok(new { qrCode });
})
.WithName("GenerateBuyerQrCode")
.WithOpenApi();

buyerGroup.MapPost("/transactions/{id}/cancel", async (HttpContext context, Guid id, IBuyerBffService buyerService) =>
{
    var userId = GetUserIdFromContext(context);
    if (!userId.HasValue)
        return Results.Unauthorized();
        
    var result = await buyerService.CancelTransactionAsync(userId.Value, id);
    return result ? Results.Ok() : Results.BadRequest();
})
.WithName("CancelBuyerTransaction")
.WithOpenApi();

buyerGroup.MapGet("/stores", async (string category, IBuyerBffService buyerService) =>
{
    var stores = await buyerService.FindStoresByCategoryAsync(category);
    return Results.Ok(stores);
})
.WithName("FindStoresByCategory")
.WithOpenApi();

// Seller endpoints
var sellerGroup = app.MapGroup("/api/seller").RequireAuthorization();

sellerGroup.MapPost("/transactions", async (HttpContext context, TransactionRequestDto request, ISellerBffService sellerService) =>
{
    var userId = GetUserIdFromContext(context);
    if (!userId.HasValue)
        return Results.Unauthorized();
        
    var result = await sellerService.ProcessTransactionAsync(userId.Value, request);
    return result.Success ? Results.Ok(result) : Results.BadRequest(result);
})
.WithName("ProcessTransaction")
.WithOpenApi();

sellerGroup.MapPost("/transactions/{id}/return", async (HttpContext context, Guid id, ISellerBffService sellerService) =>
{
    var userId = GetUserIdFromContext(context);
    if (!userId.HasValue)
        return Results.Unauthorized();
        
    var result = await sellerService.ConfirmTransactionReturnAsync(userId.Value, id);
    return result ? Results.Ok() : Results.BadRequest();
})
.WithName("ConfirmTransactionReturn")
.WithOpenApi();

sellerGroup.MapGet("/buyers/{id}/balance", async (Guid id, ISellerBffService sellerService) =>
{
    var balance = await sellerService.GetBuyerBonusBalanceAsync(id);
    return Results.Ok(new { buyerId = id, balance });
})
.WithName("GetBuyerBonusBalance")
.WithOpenApi();

sellerGroup.MapGet("/stores/{id}/balance", async (Guid id, ISellerBffService sellerService) =>
{
    var balance = await sellerService.GetStoreBonusBalanceAsync(id);
    return Results.Ok(new { storeId = id, balance });
})
.WithName("GetStoreBonusBalance")
.WithOpenApi();

sellerGroup.MapGet("/stores/{id}/transactions", async (Guid id, ISellerBffService sellerService) =>
{
    var transactions = await sellerService.GetStoreBonusTransactionsAsync(id);
    return Results.Ok(transactions);
})
.WithName("GetStoreBonusTransactions")
.WithOpenApi();

// Admin endpoints
var adminGroup = app.MapGroup("/api/admin").RequireAuthorization();

adminGroup.MapPost("/companies", async (HttpContext context, CompanyRegistrationDto request, IAdminBffService adminService) =>
{
    var userId = GetUserIdFromContext(context);
    if (!userId.HasValue)
        return Results.Unauthorized();
        
    // Verify user is an admin
    try
    {
        await adminService.GetPermittedActionsAsync(userId.Value);
    }
    catch
    {
        return Results.Forbid();
    }
        
    var result = await adminService.RegisterCompanyAsync(request);
    return result.Success ? Results.Ok(result) : Results.BadRequest(result);
})
.WithName("RegisterCompany")
.WithOpenApi();

adminGroup.MapPut("/companies/{id}/status", async (HttpContext context, Guid id, CompanyStatus status, IAdminBffService adminService) =>
{
    var userId = GetUserIdFromContext(context);
    if (!userId.HasValue)
        return Results.Unauthorized();
    
    // Verify user is an admin
    try
    {
        await adminService.GetPermittedActionsAsync(userId.Value);
    }
    catch
    {
        return Results.Forbid();
    }
        
    var result = await adminService.UpdateCompanyStatusAsync(id, status);
    return result ? Results.Ok() : Results.BadRequest();
})
.WithName("UpdateCompanyStatus")
.WithOpenApi();

adminGroup.MapPut("/stores/{id}/moderate", async (HttpContext context, Guid id, bool isApproved, IAdminBffService adminService) =>
{
    var userId = GetUserIdFromContext(context);
    if (!userId.HasValue)
        return Results.Unauthorized();
    
    // Verify user is an admin
    try
    {
        await adminService.GetPermittedActionsAsync(userId.Value);
    }
    catch
    {
        return Results.Forbid();
    }
        
    var result = await adminService.ModerateStoreAsync(id, isApproved);
    return result ? Results.Ok() : Results.BadRequest();
})
.WithName("ModerateStore")
.WithOpenApi();

adminGroup.MapPut("/companies/{id}/credit", async (HttpContext context, Guid id, decimal amount, IAdminBffService adminService) =>
{
    var userId = GetUserIdFromContext(context);
    if (!userId.HasValue)
        return Results.Unauthorized();
    
    // Verify user is an admin
    try
    {
        await adminService.GetPermittedActionsAsync(userId.Value);
    }
    catch
    {
        return Results.Forbid();
    }
        
    var result = await adminService.CreditCompanyBalanceAsync(id, amount);
    return result ? Results.Ok() : Results.BadRequest();
})
.WithName("CreditCompanyBalance")
.WithOpenApi();

adminGroup.MapGet("/transactions", async (HttpContext context, Guid? companyId, DateTime? startDate, DateTime? endDate, IAdminBffService adminService) =>
{
    var userId = GetUserIdFromContext(context);
    if (!userId.HasValue)
        return Results.Unauthorized();
    
    // Verify user is an admin
    try
    {
        await adminService.GetPermittedActionsAsync(userId.Value);
    }
    catch
    {
        return Results.Forbid();
    }
        
    var transactions = await adminService.GetSystemTransactionsAsync(companyId, startDate, endDate);
    return Results.Ok(transactions);
})
.WithName("GetSystemTransactions")
.WithOpenApi();

adminGroup.MapPost("/notifications", async (HttpContext context, Guid? recipientId, string message, NotificationType type, IAdminBffService adminService) =>
{
    var userId = GetUserIdFromContext(context);
    if (!userId.HasValue)
        return Results.Unauthorized();
    
    // Verify user is an admin
    try
    {
        await adminService.GetPermittedActionsAsync(userId.Value);
    }
    catch
    {
        return Results.Forbid();
    }
        
    var result = await adminService.SendSystemNotificationAsync(recipientId, message, type);
    return result ? Results.Ok() : Results.BadRequest();
})
.WithName("SendSystemNotification")
.WithOpenApi();

// Observer endpoints
var observerGroup = app.MapGroup("/api/observer").RequireAuthorization();

observerGroup.MapGet("/statistics", async (HttpContext context, Guid? companyId, DateTime? startDate, DateTime? endDate, IObserverBffService observerService) =>
{
    var userId = GetUserIdFromContext(context);
    if (!userId.HasValue)
        return Results.Unauthorized();
    
    // Create query object
    var query = new StatisticsQueryDto
    {
        CompanyId = companyId,
        StartDate = startDate,
        EndDate = endDate
    };
    
    var statistics = await observerService.GetStatisticsAsync(query);
    return Results.Ok(statistics);
})
.WithName("GetStatistics")
.WithOpenApi();

observerGroup.MapGet("/transactions/summary", async (HttpContext context, Guid? companyId, IObserverBffService observerService) =>
{
    var userId = GetUserIdFromContext(context);
    if (!userId.HasValue)
        return Results.Unauthorized();
        
    var summary = await observerService.GetTransactionSummaryAsync(companyId);
    return Results.Ok(summary);
})
.WithName("GetTransactionSummary")
.WithOpenApi();

observerGroup.MapGet("/companies", async (HttpContext context, IObserverBffService observerService) =>
{
    var userId = GetUserIdFromContext(context);
    if (!userId.HasValue)
        return Results.Unauthorized();
        
    var companies = await observerService.GetCompaniesOverviewAsync();
    return Results.Ok(companies);
})
.WithName("GetCompaniesOverview")
.WithOpenApi();

// Helper method to get user ID from the authenticated context
Guid? GetUserIdFromContext(HttpContext context)
{
    // For the prototype, we'll use a simple approach
    // In a real application, this would use the authenticated user's claims
    var userIdClaim = context.User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
    if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
    {
        return null;
    }
    
    return userId;
}

app.Run();