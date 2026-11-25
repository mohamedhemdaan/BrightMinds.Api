
using BrightMinds.Core.Models;
using BrightMinds.Services.Dtos.CategoryDtos;
using BrightMinds.Services.Dtos.FinalProductDtos;
using BrightMinds.Services.Dtos.IdentityDtos;
using BrightMinds.Services.Dtos.ProductDtos;
using BrightMinds.Services.Dtos.RawMaterialDtos;
using BrightMinds.Services.Dtos.RecipeDtos;
using BrightMinds.Services.Dtos.RecipeIngrediantDtos;
using BrightMinds.Services.Dtos.SalesWarehouseWithdrawRequestDtos;
using BrightMinds.Services.Dtos.SupplyItemDtos;
using BrightMinds.Services.Dtos.UnitDtos;
using BrightMinds.Services.Dtos.WarehouseDtos;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BrightMinds.Services.ApplicationServicesConfigrations
{
    public static class AddProfiles
    {

        public static IServiceCollection AddApplicationProfiles(this IServiceCollection services, IConfiguration configuration)
        {
            var baseUrl = configuration["ApiBaseUrl"];
            TypeAdapterConfig<RawMaterial, RawMaterialDetailsDto>
                .NewConfig()
                .Map(dest => dest.UnitType, src => src.UnitType.ToString())
                 .Map(dest => dest.ContainerUnitName, src => src.ContainerUnitName)
                 .Map(dest => dest.ContainerSize, src => src.ContainerSize)
                 .Map(dest => dest.UnitWeightInKg, src => src.UnitWeightInKg)
                 .Map(dest => dest.IsWeightBased, src => src.IsWeightBased)
                 .Map(dest => dest.TotalWeightPerContainer, src => src.TotalWeightPerContainer)
                .Map(dest => dest.CategoryName, src => src.Category != null ? src.Category.Name : null)
                .Map(dest => dest.HandlingType, src => src.HandlingType)
                                 .Map(dest => dest.SubUnitName, src => src.SubUnitName)
                                 .Map(dest => dest.TotalContainerPrice, src => src.TotalContainerPrice)
                                 .Map(dest => dest.PricePerKg, src => src.PricePerKg)
                                 .Map(dest => dest.PricePerPiece, src => src.PricePerPiece)
                                     .Map(dest => dest.InnerUnitName, src => src.InnerUnitName)
                    .Map(dest => dest.InnerUnitsPerSubUnit, src => src.InnerUnitsPerSubUnit)
                    .Map(dest => dest.InnerUnitWeightInKg, src => src.InnerUnitWeightInKg)
                    .Map(dest => dest.PricePerInnerUnit, src => src.PricePerInnerUnit);
            ;

            //.Map(dest => dest.PricePerUnit, src => src.PricePerUnit);

            TypeAdapterConfig<Category, CategoryDetailsDto>.NewConfig();
            TypeAdapterConfig<Category, CategoryDto>.NewConfig();

            TypeAdapterConfig<RecipeIngrediant, RecipeIngrediantDetailsDto>
                .NewConfig()
                .Map(rI => rI.RawMaterialName, r => r.RawMaterial.Name)
                .Map(rI => rI.SubRecipeName, r => r.SubRecipe.Name);

            TypeAdapterConfig<WarehouseSuppliesInventory, WarehouseInventoryItemDetailsDto>.NewConfig()
                .Map(dist => dist.InventoryItemName, src => src.SupplyItem.RawMaterial.Name)
                .Map(dist => dist.Price, src => src.Price)
                .Map(dist => dist.Quantity, src => src.Quantity)
                .Map(dist => dist.UnitType, src => src.UnitType)
                .Map(dist => dist.CategoryId, src => src.SupplyItem.RawMaterial.CategoryId)
                .Map(dist => dist.CategoryName, src => src.SupplyItem.RawMaterial.Category.Name)
                 .Map(dist => dist.ContainerQuantity, src => src.ContainerQuantity)
                .Map(dist => dist.ContainerUnitName, src => src.SupplyItem.RawMaterial.ContainerUnitName)
                .Map(dist => dist.DisplayDetails, src => src.DisplayDetails)
                 .Map(dist => dist.SubUnitName, src => src.SupplyItem.RawMaterial.SubUnitName)
                 .Map(dist => dist.TotalWeightInKg, src => src.SupplyItem.RawMaterial.UnitWeightInKg.HasValue ? src.Quantity * src.SupplyItem.RawMaterial.UnitWeightInKg.Value : (decimal?)null)
                 .Map(dist => dist.HandlingType, src => src.SupplyItem.RawMaterial.HandlingType)
                     .Map(dist => dist.ContainerSize, src => src.SupplyItem.RawMaterial.ContainerSize)
                   .Map(dist => dist.ContainerPrice, src => src.Price * src.SupplyItem.RawMaterial.ContainerSize);

            TypeAdapterConfig<SupplyItem, WarehouseSuppliesInventory>.NewConfig()
                .Map(dist => dist.SupplyItemId, src => src.Id)
                .Map(dist => dist.Quantity, src => src.Quantity)
                .Map(dist => dist.Price, src => src.UnitPrice)
                .Ignore(dist => dist.Id);

            TypeAdapterConfig<RecipeIngrediant, ProductIngredientSnapshoot>.NewConfig()
                .Map(dist => dist.Name, src => src.RawMaterial.Name)
                .Map(dist => dist.Quantity, src => src.Quantity)
                .Map(dist => dist.UnitType, src => src.UnitType)
                .Ignore(dist => dist.Id)
                .Ignore(dist => dist.ProductId)
                .Ignore(dist => dist.Price);

            TypeAdapterConfig<WarehouseProductsInventory, WarehouseProductDetailsDto>.NewConfig()
                .Map(dist => dist.Id, src => src.Id)
                .Map(dist => dist.Name, src => src.ProductPortion.Name)
                .Map(dist => dist.Quantity, src => src.Quantity)
                .Map(dist => dist.SizeIngrams, src => src.Size);

            TypeAdapterConfig<RequestedItemDto, RequestedSalesWarehouseInventoryItem>.NewConfig()
                .Map(dist => dist.InventoryItemId, src => src.InventoryItemId)
                .Map(dist => dist.Quantity, src => src.Quantity);

            TypeAdapterConfig<SalesWarehouseWithdrawRequest, SalesWarehouseWithdrawRequestDetailsDto>.NewConfig()
                .Map(dist => dist.Date, src => src.CreationDate)
                .Map(dist => dist.Status, src => src.Status)
                .Map(dist => dist.Status, src => src.Status);

            TypeAdapterConfig<RequestedSalesWarehouseInventoryItem, RequestedItemDetailsDto>.NewConfig()
                .Map(dist => dist.InventoryItemId, src => src.InventoryItemId)
                .Map(dist => dist.Quantity, src => src.Quantity)
                .Map(dist => dist.Name, src => src.ProductPortion.Name)
                .Map(dist => dist.Size, src => src.ProductPortion.SizeInGrams);

            TypeAdapterConfig<AppUser, UserDto>.NewConfig()
                .Map(dist => dist.DisplayName, src => src.FirstName + " " + src.LastName);

            TypeAdapterConfig<BaseUnit, BaseUnitDetailsDto>.NewConfig()
                .Map(dist => dist.CategoryName, src => src.Category.Name);

            // services.AddAutoMapper(m => m.AddProfile(new StoryProfile(configuration)));

            //TypeAdapterConfig<SupplyItem, SupplyItemDetailsDto>.NewConfig()
            //     .Map(dest => dest.ContainerQuantity, src => src.ContainerQuantity)
            //     .Map(dest => dest.Quantity, src => src.Quantity)
            //     .Map(dest => dest.DisplayDetails, src => src.DisplayDetails)
            //     .Map(dest => dest.TotalPrice, src => src.Quantity * src.UnitPrice)
            //     .Map(dest => dest.UnitType, src => src.UnitType)
            //     .Map(dest => dest.RawMaterialId, src => src.RawMaterialId)
            //     .Map(dest => dest.SupplyId, src => src.SupplyId);

            TypeAdapterConfig<SupplyItem, SupplyItemDetailsDto>.NewConfig()
    .Map(dest => dest.ContainerQuantity, src => src.ContainerQuantity)
    .Map(dest => dest.Quantity, src => src.Quantity)
    .Map(dest => dest.DisplayDetails, src => src.DisplayDetails)
    .Map(dest => dest.TotalPrice, src => src.Quantity * src.UnitPrice)
    .Map(dest => dest.UnitType, src => src.UnitType)
    .Map(dest => dest.RawMaterialId, src => src.RawMaterialId)
    .Map(dest => dest.SupplyId, src => src.SupplyId)
    .Map(dest => dest.ReceivedBy, src => src.ReceivedBy)
    .Map(dest => dest.OriginalQuantity, src => src.OriginalQuantity)
    .Map(dest => dest.OriginalPrice, src => src.OriginalPrice);

            TypeAdapterConfig<Recipe, RecipeDetailsDto>.NewConfig()
              .Map(dest => dest.PortionCategoryName, src => src.PortionCategory != null ? src.PortionCategory.Name : null);

            TypeAdapterConfig<FinalProduct, FinalProductDetailsDto>.NewConfig()
              .Map(dest => dest.CategoryName, src => src.Category != null ? src.Category.Name : null)
              .Map(dest => dest.SubCategoryName, src => src.SubCategory != null ? src.SubCategory.Name : null); ;

            TypeAdapterConfig<Product, ProductDetailsDto>.NewConfig()
     .Map(dest => dest.SettledByUserName, src =>
         src.SettledByUser != null ? $"{src.SettledByUser.FirstName} {src.SettledByUser.LastName}" : null);

            return services;
        }
    }
}
