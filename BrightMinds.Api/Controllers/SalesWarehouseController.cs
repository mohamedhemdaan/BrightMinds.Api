using BrightMinds.Services.Dtos.WarehouseDtos;
using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightMinds.Api.Controllers
{
    public class SalesWarehouseController : ApiBaseController
    {
        private readonly IWarehouseService _warehouseService;
        public SalesWarehouseController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpGet("Inventory")]
        [Authorize(Roles = "Admin,WarehouseManager,NightShiftWarehouseClerk,MorningShiftWarehouseClerk,NightShiftChef,MorningShiftChef")]
        public async Task<IActionResult> GetAll([FromQuery] WarehouseProductsSpecParams specParams)
        {
            var result = await _warehouseService.GetAllPrductsInventoryAsync(specParams);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("Withdraw")]
        [Authorize(Roles = "Admin,WarehouseManager,NightShiftWarehouseClerk,MorningShiftWarehouseClerk")]
        public async Task<IActionResult> Withdraw(SalesInventoryWithdrawDto withdrawDto)
        {
            var result = await _warehouseService.WithdrawFromSales(withdrawDto);
            return StatusCode(result.StatusCode, result);
        }
    }
}
