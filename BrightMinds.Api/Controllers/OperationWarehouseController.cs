using BrightMinds.Api.Response;
using BrightMinds.Core.Interfaces;
using BrightMinds.Services.Dtos.ProductDtos;
using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BrightMinds.Api.Controllers
{
    [Authorize(Roles = "Admin,OperationChef")]
    public class OperationWarehouseController : ApiBaseController
    {
        private readonly IWarehouseService _warehouseService;
        private readonly IUnitOfWork _unitOfWork; 
        public OperationWarehouseController(IWarehouseService warehouseService, IUnitOfWork unitOfWork)
        {
            _warehouseService = warehouseService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("production")]
        public async Task<IActionResult> GetAllProductions([FromQuery] ProductionSpecParams specParams)
        {
            var result = await _warehouseService.GetAllProductions(specParams);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("Produce/{recipeId}")]
        public async Task<IActionResult> ProduceRecipie([FromRoute]int recipeId)
        {
            using(var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var result = await _warehouseService.Produce(recipeId);

                    if(result.StatusCode != (int)HttpStatusCode.OK)
                    {
                        await _unitOfWork.RollBackAsync();
                        return StatusCode(result.StatusCode, result);
                    }

                    await _unitOfWork.CompleteAsync();
                    await _unitOfWork.CommitAsync();

                    result.Data = result.Data.Adapt<ProductDetailsDto>();
                    return StatusCode(result.StatusCode, result);
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollBackAsync();
                    var result = new ApiResponse((int)HttpStatusCode.InternalServerError, message: "حدث خطأ اثناء انتاج الوصفة");
                    return StatusCode(result.StatusCode, result);
                }
            }

        }

        [HttpPost("actual-quantities")]
        public async Task<IActionResult> UpdateActualQuantaties([FromBody] ProductQuantitiesDto Quantities)
        {
            var result = await _warehouseService.UpdateActualProductQuantities(Quantities);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{productId}/transfere-product-to-sales")]
        public async Task<IActionResult> TransfereproductToSales(int productId)
        {
            var result = await _warehouseService.TransfereProductToSalesWarehouse(productId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{productPortionId}/transfere-product-portion-to-sales")]
        public async Task<IActionResult> TransfereproductPortionToSales(int productPortionId)
        {
            var result = await _warehouseService.TransfereProductPortionToSalesWarehouse(productPortionId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("Production/{productId}")]
        [Authorize(Roles = "Admin,OperationChef")]
        public async Task<IActionResult> GetProductionDetails(int productId)
        {
            var product = await _unitOfWork.ProductRepository.GetAsync(productId);
            if (product == null)
                return NotFound(new ApiResponse(404, "المنتج غير موجود"));

            var result = product.Adapt<ProductDetailsDto>();
            return Ok(new { statusCode = 200, message = "تم جلب التفاصيل", data = result });
        }

        [HttpPost("{productId}/settle")]
        [Authorize(Roles = "Admin,OperationChef")]
        public async Task<IActionResult> SettleProduct(int productId, [FromBody] decimal settledWeightInKg)
        {
            var result = await _warehouseService.SettleProductAsync(productId, settledWeightInKg);
            return StatusCode(result.StatusCode, result);
        }
    }
}
