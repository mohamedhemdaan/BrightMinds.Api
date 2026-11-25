
using BrightMinds.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Blendd.Infrastructure.DataAccess
{
    public class AppDbContext:IdentityDbContext<AppUser>
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
 
        public DbSet<RawMaterial> RawMaterials { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Supply> Supplies { get; set; }
        public DbSet<SupplyItem> SupplyItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipePortion> RecipePortions { get; set; }
        public DbSet<RecipeIngrediant> RecipeIngrediants { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<WarehouseSuppliesInventory> WarehouseSuppliesInventory { get; set; }
        public DbSet<WithdrawHistory> WithdrawHistory { get; set; }
        public DbSet<WithdrawItem> WithdrawItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductIngredientSnapshoot> ProductIngredientsSnapshoots { get; set; }
        public DbSet<WarehouseProductsInventory> WarehouseProductsInventorys { get; set; }
        public DbSet<ProductPortions> ProductPortions { get; set; }
        public DbSet<SalesWarehouseWithdrawRequest> SalesWarehouseWithdrawRequests { get; set; }
        public DbSet<RequestedSalesWarehouseInventoryItem> RequestedSalesWarehouseInventoryItems { get; set; }
        public DbSet<FinalProduct> FinalProducts { get; set; }
        public DbSet<FinalProductIngredient> FinalProductIngredients { get; set; }
        public DbSet<RawMaterialIngredient> RawMaterialIngredients { get; set; }
        public DbSet<PortionIngredient> PortionIngredients { get; set; }
        public DbSet<FinalProductTax> FinalProductTaxs { get; set; }
        public DbSet<SupplyReturn> SupplyReturns { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<BaseUnit> BaseUnits { get; set; }
        public DbSet<ComplexUnit> ComplexUnits { get; set; }
        public DbSet<RawMaterialUnit> RawMaterialUnits { get; set; }
        public DbSet<UomCategory> UomCategories { get; set; }

        public DbSet<MiscellaneousItem> MiscellaneousItems { get; set; }
        public DbSet<MiscellaneousWithdraw> MiscellaneousWithdraws { get; set; }
        public DbSet<MiscellaneousAddition> MiscellaneousAdditions { get; set; }

        public DbSet<RecipeCategory> RecipeCategories { get; set; }

        public DbSet<PortionCategory> PortionCategories { get; set; }


        public DbSet<FinalProductCategory> FinalProductCategories { get; set; }
        public DbSet<FinalProductSubCategory> FinalProductSubCategories { get; set; }
        public DbSet<RestaurantSupplyWithdraw> RestaurantSupplyWithdraws { get; set; }

    }
}
