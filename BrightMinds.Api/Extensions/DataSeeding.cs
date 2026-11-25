using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Enums;
using BrightMinds.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Extensions
{
    public static class DataSeeding
    {
        public static async Task<WebApplication> SeedAppData(this WebApplication application, AppDbContext context)
        {
            //Populating The WareHouses
            if (!context.Warehouses.Any())
            {
                var mainWarehouse = new Warehouse()
                {
                    Name = "Main Warehouse",
                    Type = WarehouseType.MainWarehouse
                };

                var subWarehouse = new Warehouse()
                {
                    Name = "Sub Warehouse",
                    Type = WarehouseType.SubWarehouse
                };

                var oprationalWarehouse = new Warehouse()
                {
                    Name = "Operational Warehouse",
                    Type = WarehouseType.OperationalWarehouse
                };

                var salesWarehouse = new Warehouse()
                {
                    Name = "Sales Warehouse",
                    Type = WarehouseType.SalesWarehouse
                };

                await context.Warehouses.AddAsync(mainWarehouse);
                await context.Warehouses.AddAsync(subWarehouse);
                await context.Warehouses.AddAsync(oprationalWarehouse);
                await context.Warehouses.AddAsync(salesWarehouse);
                await context.SaveChangesAsync();
            }

            if (!context.FinalProductTaxs.Any())
            {
                await context.FinalProductTaxs.AddAsync(new FinalProductTax()
                {
                    TaxPrecentage = 14
                });
                await context.SaveChangesAsync();
            }

            // Seeding the Categories of units => Weight, Volume, Quantity, and the Base Units => Gram, Kilo, ML, L, Peice
            if (!await context.UomCategories.AnyAsync())
            {
                var weight = new UomCategory() { Name = "Weight" };
                var volume = new UomCategory() { Name = "Volume" };
                var quantity = new UomCategory() { Name = "Quantity" };

                await context.UomCategories.AddRangeAsync(weight, volume, quantity);

                var gram = new BaseUnit()
                {
                    Category = weight,
                    IsImmutable = true,
                    ConversionFactor = 1,
                    Name = "جرام",
                    Type = UomType.Base
                };

                var kilo = new BaseUnit()
                {
                    Category = weight,
                    IsImmutable = true,
                    ConversionFactor = 1000,
                    Name = "كيلوجرام",
                    Type = UomType.Base
                };

                var milliliters = new BaseUnit()
                {
                    Category = volume,
                    IsImmutable = true,
                    ConversionFactor = 1,
                    Name = "ميلي لتر",
                    Type = UomType.Base
                };

                var litir = new BaseUnit()
                {
                    Category = volume,
                    IsImmutable = true,
                    ConversionFactor = 1000,
                    Name = "لتر",
                    Type = UomType.Base
                };

                var peice = new BaseUnit()
                {
                    Category = quantity,
                    IsImmutable = true,
                    ConversionFactor = 1,
                    Name = "قطعة",
                    Type = UomType.Base
                };

                await context.Units.AddRangeAsync(gram, kilo, milliliters, litir, peice);
                await context.SaveChangesAsync();
            }

            
                return application;
        }
    }
}