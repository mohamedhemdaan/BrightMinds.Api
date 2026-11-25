using BrightMinds.Core.Interfaces;
using BrightMinds.Infrastructure.DataAccess;
using BrightMinds.Infrastructure.Repositories;
using BrightMinds.Services.IServices;
using BrightMinds.Services.Services;
using Microsoft.Extensions.DependencyInjection;


namespace BrightMinds.Services.ApplicationServicesConfigrations
{
    public static class AddRepositories
    {
        public static IServiceCollection AddApplicationRepositories(this IServiceCollection services)
        {

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IRawMaterialRepository, RawMaterialRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ISupplyRepository, SupplyRepository>();
            services.AddScoped<ISupplyItemRepository, SupplyItemRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IRecipeRepository, RecipeRepository>();
            services.AddScoped<IWarehouseRepository, WarehouseRepository>();
            services.AddScoped<IWarehouseSuppliesInventoryRepository, WarehouseSuppliesInventoryRepository>();
            services.AddScoped<IWithdrawHistoryRepository, WithdrawHistoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IWarehouseProductsInventoryRepository, WarehouseProductsInventoryRepository>();
            services.AddScoped<ISalesWarehouseWithdrawRequestRepository, SalesWarehouseWithdrawRequestRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<ISupplyInstallmentRepository, SupplyInstallmentRepository>();
            services.AddScoped<IFinalProductRepository, FinalProductRepository>();
            services.AddScoped<IFinalProductTaxRepository, FinalProductTaxRepository>();
            services.AddScoped<ISupplyReturnRepository, SupplyReturnRepository>();
            services.AddScoped<IShiftRepository, ShiftRepository>();

            services.AddScoped<IMiscellaneousService, MiscellaneousService>();

            return services;
        }
    }
}
