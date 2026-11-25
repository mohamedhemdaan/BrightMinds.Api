using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BrightMinds.Core.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IRawMaterialRepository RawMaterialRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        ISupplyRepository SupplyRepository { get; }
        ISupplyItemRepository SupplyItemRepository { get; }
        ISupplierRepository SupplierRepository { get; }
        IRecipeRepository RecipeRepository { get; }
        IWarehouseRepository WarehouseRepository { get; }
        IWarehouseSuppliesInventoryRepository WarehouseSuppliesInventoryRepository { get; }
        IWithdrawHistoryRepository WithdrawHistoryRepository { get; }
        IProductRepository ProductRepository { get; }
        IProductIngredientsSnapshootRepository ProductIngredientsSnapshootRepository { get; }
        IWarehouseProductsInventoryRepository WarehouseProductsInventoryRepository { get; }
        IProductPortionRepository ProductPortionRepository { get; }
        ISalesWarehouseWithdrawRequestRepository SalesWarehouseWithdrawRequestRepository { get; }
        IReportRepository ReportRepository { get; }
        IRecipePortionRepository RecipePortionRepository { get; }
        ISupplyInstallmentRepository SupplyInstallmentRepository { get; }
        IFinalProductRepository FinalProductRepository { get; }
        IFinalProductTaxRepository FinalProductTaxRepository { get; }
        ISupplyReturnRepository SupplyReturnRepository { get; }
        IShiftRepository ShiftRepository { get; }
        IUnitRepository UnitRepository { get; }
        IUomCategoryRepository UomCategoryRepository { get; }

        IBaseUnitRepository BaseUnitRepository { get; }
        IComplexUnitRepository ComplexUnitRepository { get; }

        IMiscellaneousItemRepository MiscellaneousItemRepository { get; }
        IMiscellaneousWithdrawRepository MiscellaneousWithdrawRepository { get; }

        IMiscellaneousAdditionRepository MiscellaneousAdditionRepository { get; }

        IRecipeCategoryRepository RecipeCategoryRepository { get; }

        IPortionCategoryRepository PortionCategoryRepository { get; }

       IFinalProductCategoryRepository FinalProductCategoryRepository {get;}

         IFinalProductSubCategoryRepository FinalProductSubCategoryRepository { get;}
        IRequestedSalesWarehouseInventoryItemRepository RequestedSalesWarehouseInventoryItemRepository { get;}
        IRestaurantSupplyWithdrawRepository RestaurantSupplyWithdrawRepository { get; }
        Task RollBackAsync();
        Task CommitAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<int> CompleteAsync();
    }
}
