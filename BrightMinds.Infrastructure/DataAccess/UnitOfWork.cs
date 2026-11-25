using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Interfaces;
using BrightMinds.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BrightMinds.Infrastructure.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

      private readonly Lazy<IRawMaterialRepository> _rawMaterialRepository;
      private readonly Lazy<ICategoryRepository> _categoryRepository;
      private readonly Lazy<ISupplyRepository> _supplyRepository;
      private readonly Lazy<ISupplyItemRepository> _supplyItemRepository;
      private readonly Lazy<ISupplierRepository> _supplierRepository;
      private readonly Lazy<IRecipeRepository> _recipeRepository;
      private readonly Lazy<IWarehouseRepository> _warehouseRepository;
      private readonly Lazy<IWarehouseSuppliesInventoryRepository> _warehouseSuppliesInventoryRepository;
      private readonly Lazy<IWithdrawHistoryRepository> _withdrawHistoryRepository;
      private readonly Lazy<IProductIngredientsSnapshootRepository> _productIngredientsSnapshootsRepository;
      private readonly Lazy<IProductRepository> _productRepository;
      private readonly Lazy<IWarehouseProductsInventoryRepository> _warehouseProductsInventoryRepository;
      private readonly Lazy<IProductPortionRepository> _productPortionRepository;
      private readonly Lazy<ISalesWarehouseWithdrawRequestRepository> _salesWarehouseWithdrawRequestRepository;
      private readonly Lazy<IReportRepository> _reportRepository;
      private readonly Lazy<IRecipePortionRepository> _recipePortionRepository;
      private readonly Lazy<ISupplyInstallmentRepository> _supplyInstallmentRepository;
      private readonly Lazy<IFinalProductRepository> _finalProductRepository;
      private readonly Lazy<IFinalProductTaxRepository> _finalProductTaxRepository;
      private readonly Lazy<ISupplyReturnRepository> _supplyReturnRepository;
      private readonly Lazy<IShiftRepository> _shiftRepository;
      private readonly Lazy<IUnitRepository> _unitRepository;
      private readonly Lazy<IUomCategoryRepository> _uomCategoryRepository;
      private readonly Lazy<IBaseUnitRepository> _baseUnitRepository;
      private readonly Lazy<IComplexUnitRepository> _complexUnitRepository;
      private readonly Lazy<IMiscellaneousItemRepository> _miscellaneousItemRepository;
      private readonly Lazy<IMiscellaneousWithdrawRepository> _miscellaneousWithdrawRepository;
      private readonly Lazy<IMiscellaneousAdditionRepository> _miscellaneousAdditionRepository;

      private readonly Lazy<IRecipeCategoryRepository> _recipeCategoryRepository;
      private readonly Lazy<IPortionCategoryRepository> _portionCategoryRepository;
      private readonly Lazy<IFinalProductCategoryRepository> _finalProductCategoryRepository;
      private readonly Lazy<IFinalProductSubCategoryRepository> _finalProductSubCategoryRepository;
      private readonly Lazy<IRequestedSalesWarehouseInventoryItemRepository> _requestedSalesWarehouseInventoryItemRepository;
      private readonly Lazy<IRestaurantSupplyWithdrawRepository> _restaurantSupplyWithdrawRepository;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;

           _rawMaterialRepository = new Lazy<IRawMaterialRepository>(() => new RawMaterialRepository(_context));
           _categoryRepository = new Lazy<ICategoryRepository>(() => new CategoryRepository(_context));
           _supplyRepository = new Lazy<ISupplyRepository>(() => new SupplyRepository(_context));
           _supplyItemRepository = new Lazy<ISupplyItemRepository>(() => new SupplyItemRepository(_context));
           _supplierRepository = new Lazy<ISupplierRepository>(() => new SupplierRepository(_context));
            _recipeRepository = new Lazy<IRecipeRepository>(() => new RecipeRepository(_context));
            _warehouseRepository = new Lazy<IWarehouseRepository>(() => new WarehouseRepository(_context));
            _withdrawHistoryRepository = new Lazy<IWithdrawHistoryRepository>(() => new WithdrawHistoryRepository(_context));
            _warehouseSuppliesInventoryRepository = new Lazy<IWarehouseSuppliesInventoryRepository>(() => new WarehouseSuppliesInventoryRepository(_context));
            _productIngredientsSnapshootsRepository = new Lazy<IProductIngredientsSnapshootRepository>(() => new ProductIngredientsSnapshootRepository(_context));
            _productRepository = new Lazy<IProductRepository>(() => new ProductRepository(_context));
            _warehouseProductsInventoryRepository = new Lazy<IWarehouseProductsInventoryRepository>(() => new WarehouseProductsInventoryRepository(_context));
            _productPortionRepository = new Lazy<IProductPortionRepository>(() => new ProductPortionRepository(_context));
            _salesWarehouseWithdrawRequestRepository = new Lazy<ISalesWarehouseWithdrawRequestRepository>(() => new SalesWarehouseWithdrawRequestRepository(_context));
            _reportRepository = new Lazy<IReportRepository>(() => new ReportRepository(_context));
            _recipePortionRepository = new Lazy<IRecipePortionRepository>(() => new RecipePortionRepository(_context));
            _supplyInstallmentRepository = new Lazy<ISupplyInstallmentRepository>(() => new SupplyInstallmentRepository(_context));
            _finalProductRepository = new Lazy<IFinalProductRepository>(() => new FinalProductRepository(_context));
            _finalProductTaxRepository = new Lazy<IFinalProductTaxRepository>(() => new FinalProductTaxRepository(_context));
            _supplyReturnRepository = new Lazy<ISupplyReturnRepository>(() => new SupplyReturnRepository(_context));
            _shiftRepository = new Lazy<IShiftRepository>(() => new ShiftRepository(_context));
            _unitRepository = new Lazy<IUnitRepository>(() => new UnitRepository(_context));
            _uomCategoryRepository = new Lazy<IUomCategoryRepository>(() => new UomCategoryRepository(_context));
            _baseUnitRepository = new Lazy<IBaseUnitRepository>(() => new BaseUnitRepository(_context));
            _complexUnitRepository = new Lazy<IComplexUnitRepository>(() => new ComplexUnitRepository(_context));

            _miscellaneousItemRepository = new Lazy<IMiscellaneousItemRepository>(() => new MiscellaneousItemRepository(_context));
            _miscellaneousWithdrawRepository = new Lazy<IMiscellaneousWithdrawRepository>(() => new MiscellaneousWithdrawRepository(_context));
            _miscellaneousAdditionRepository = new Lazy<IMiscellaneousAdditionRepository>(() => new MiscellaneousAdditionRepository(_context));

            _recipeCategoryRepository = new Lazy<IRecipeCategoryRepository>(() => new RecipeCategoryRepository(_context));
            _portionCategoryRepository = new Lazy<IPortionCategoryRepository>(() => new PortionCategoryRepository(_context));
            _finalProductCategoryRepository = new Lazy<IFinalProductCategoryRepository>(() => new FinalProductCategoryRepository(_context));
            _finalProductSubCategoryRepository = new Lazy<IFinalProductSubCategoryRepository>(() => new FinalProductSubCategoryRepository(_context));
            _requestedSalesWarehouseInventoryItemRepository = new Lazy<IRequestedSalesWarehouseInventoryItemRepository>(() => new RequestedSalesWarehouseInventoryItemRepository(_context));
            _restaurantSupplyWithdrawRepository = new Lazy<IRestaurantSupplyWithdrawRepository>(() => new RestaurantSupplyWithdrawRepository(_context));

        }

        public IRawMaterialRepository RawMaterialRepository => _rawMaterialRepository.Value;
        public ICategoryRepository CategoryRepository => _categoryRepository.Value;
        public ISupplyRepository SupplyRepository => _supplyRepository.Value;
        public ISupplyItemRepository SupplyItemRepository => _supplyItemRepository.Value;
        public ISupplierRepository SupplierRepository => _supplierRepository.Value;
        public IRecipeRepository RecipeRepository => _recipeRepository.Value;
        public IWarehouseRepository WarehouseRepository => _warehouseRepository.Value;
        public IWithdrawHistoryRepository WithdrawHistoryRepository => _withdrawHistoryRepository.Value;
        public IWarehouseSuppliesInventoryRepository WarehouseSuppliesInventoryRepository => _warehouseSuppliesInventoryRepository.Value;
        public IProductIngredientsSnapshootRepository ProductIngredientsSnapshootRepository => _productIngredientsSnapshootsRepository.Value;
        public IProductRepository ProductRepository => _productRepository.Value;
        public IWarehouseProductsInventoryRepository WarehouseProductsInventoryRepository => _warehouseProductsInventoryRepository.Value;
        public IProductPortionRepository ProductPortionRepository => _productPortionRepository.Value;
        public ISalesWarehouseWithdrawRequestRepository SalesWarehouseWithdrawRequestRepository => _salesWarehouseWithdrawRequestRepository.Value;
        public IReportRepository ReportRepository => _reportRepository.Value;
        public IRecipePortionRepository RecipePortionRepository => _recipePortionRepository.Value;
        public ISupplyInstallmentRepository SupplyInstallmentRepository => _supplyInstallmentRepository.Value;
        public IFinalProductRepository FinalProductRepository => _finalProductRepository.Value;
        public IFinalProductTaxRepository FinalProductTaxRepository => _finalProductTaxRepository.Value;
        public ISupplyReturnRepository SupplyReturnRepository => _supplyReturnRepository.Value;
        public IShiftRepository ShiftRepository => _shiftRepository.Value;
        public IUnitRepository UnitRepository => _unitRepository.Value;
        public IUomCategoryRepository UomCategoryRepository => _uomCategoryRepository.Value;
        public IBaseUnitRepository BaseUnitRepository => _baseUnitRepository.Value;
        public IComplexUnitRepository ComplexUnitRepository => _complexUnitRepository.Value;

        public IMiscellaneousItemRepository MiscellaneousItemRepository => _miscellaneousItemRepository.Value;
        public IMiscellaneousWithdrawRepository MiscellaneousWithdrawRepository => _miscellaneousWithdrawRepository.Value;
        public IMiscellaneousAdditionRepository MiscellaneousAdditionRepository => _miscellaneousAdditionRepository.Value;
        public IRecipeCategoryRepository RecipeCategoryRepository => _recipeCategoryRepository.Value;
        public IPortionCategoryRepository PortionCategoryRepository => _portionCategoryRepository.Value;
        public IFinalProductCategoryRepository FinalProductCategoryRepository => _finalProductCategoryRepository.Value;
        public IFinalProductSubCategoryRepository FinalProductSubCategoryRepository => _finalProductSubCategoryRepository.Value;
        public IRequestedSalesWarehouseInventoryItemRepository RequestedSalesWarehouseInventoryItemRepository => _requestedSalesWarehouseInventoryItemRepository.Value;
        public IRestaurantSupplyWithdrawRepository RestaurantSupplyWithdrawRepository => _restaurantSupplyWithdrawRepository.Value;

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }

        public async Task RollBackAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }
    }

}
