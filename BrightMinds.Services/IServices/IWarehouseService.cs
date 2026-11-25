using BrightMinds.Core.Enums;
using BrightMinds.Core.Models;
using BrightMinds.Services.Dtos.ProductDtos;
using BrightMinds.Services.Dtos.WarehouseDtos;
using BrightMinds.Services.SpecificationParams;

namespace BrightMinds.Services.IServices
{
    public interface IWarehouseService
    {
        public Task<Response> GetAllWarehouses();
        public Task<Response> GetAllSuppliesInventoryAsync(WarehouseInventorySpecParams specParams, WarehouseType warehouseType);
        public Task<Response> GetAllPrductsInventoryAsync(WarehouseProductsSpecParams specParams);
        public Task<Response> WithdrawFromMain(MainInventoryWithdrawaDto withdrawlDto);
        public Task<Response> GetWithrawHistory(WithdrawHistorySpecParams specParams);
        public Task<Response> Produce(int? recipeId, bool isParent = true);
        public Task<Response> UpdateActualProductQuantities(ProductQuantitiesDto productQuantitiesDto);
        public Task<Response> TransfereProductPortionToSalesWarehouse(int productPortionId, bool saveChanges = true, WithdrawHistory withdrawHistory = null);
        public Task<Response> TransfereProductToSalesWarehouse(int productId);
        public Task<Response> GetDailyProduction();
        public Task<Response> GetAllProductions(ProductionSpecParams specParams);
        public Task<Response> WithdrawFromSales(SalesInventoryWithdrawDto withdrawDto);
        public Task<Response> SettleProductAsync(int productId, decimal settledWeightInKg);

    }
}
