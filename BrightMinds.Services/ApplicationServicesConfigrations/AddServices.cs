using BrightMinds.Services.Services;
using BrightMinds.Services.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BrightMinds.Services.ApplicationServicesConfigrations
{
    public static class AddServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddApplicationProfiles(configuration);
            services.AddApplicationRepositories();
            // services.AddSingleton<IResponseCacheService,ResponseCacheService>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(
                options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configuration["JWT:ValidIssuer"],
                        ValidateAudience = true,
                        ValidAudience = configuration["JWT:ValidAudience"],
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]))
                    };

                }

                );
            services.AddSignalR();

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IRawMaterialService, RawMaterialService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ISupplyService, SupplyService>();
            services.AddScoped<ISupplyItemService, SupplyItemService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IRecipeService, RecipeService>();
            services.AddScoped<IWarehouseService, WarehouseService>();
            services.AddScoped<ISalesWarehouseWithdrawRequestService, SalesWarehouseWithdrawRequestService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IFinalProductService, FinalProductService>();
            services.AddScoped<ISupplyReturnService, SupplyReturnService>();
            services.AddScoped<IShiftService, ShiftService>();
            services.AddScoped<IUnitService, UnitService>();
            services.AddScoped<IBaseUnitService, BaseUnitService>();
            services.AddScoped<IComplexUnitService, ComplexUnitService>();
            services.AddScoped<IRecipeCategoryService, RecipeCategoryService>();

            services.AddScoped<IPortionCategoryService, PortionCategoryService>();
            services.AddScoped<IFinalProductCategoryService, FinalProductCategoryService>();
            services.AddScoped<IFinalProductSubCategoryService, FinalProductSubCategoryService>();
            services.AddScoped<IRestaurantSupplyService, RestaurantSupplyService>();
            return services;
        }
    }   
}
