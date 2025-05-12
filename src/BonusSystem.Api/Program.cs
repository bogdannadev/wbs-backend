using BonusSystem.Api.Features.Admin;
using BonusSystem.Api.Features.Auth;
using BonusSystem.Api.Features.Buyers;
using BonusSystem.Api.Features.Companies;
using BonusSystem.Api.Features.Observers;
using BonusSystem.Api.Features.Sellers;
using BonusSystem.Api.Features.Accounting;
using BonusSystem.Api.Infrastructure.Extensions;

public class Program
{
    public static async Task Main(string[] args)
    {
        // // Set EPPlus license using LicenseContext property
        // ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        var builder = WebApplication.CreateBuilder(args);

        // Configure services
        builder.Services.AddApiServices(builder.Configuration);

        var app = builder.Build();

        // Configure middleware
        app.UseApiMiddleware(app.Environment);

        // Map endpoints by feature
        app.MapAuthEndpoints();
        app.MapBuyerEndpoints();
        app.MapSellerEndpoints();
        app.MapAdminEndpoints();
        app.MapCompanyEndpoints();
        app.MapObserverEndpoints();
        app.MapAccountingEndpoints();

        await app.RunAsync();
    }
}