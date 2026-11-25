using BrightMinds.Core.Enums;
using BrightMinds.Services.Dtos.WarehouseDtos;
using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightMinds.Api.Controllers
{
    public class SubWarehouseController : ApiBaseController
    {
        private readonly IWarehouseService _warehouseService;

        public SubWarehouseController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }
            
        [HttpGet("Inventory")]
        [Authorize(Roles = "Admin,WarehouseManager,NightShiftWarehouseClerk,MorningShiftWarehouseClerk")]
        public async Task<IActionResult> GetInventory([FromQuery] WarehouseInventorySpecParams specParams)
        {
            var result = await _warehouseService.GetAllSuppliesInventoryAsync(specParams, WarehouseType.SubWarehouse);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("Withdraw")]
        [Authorize(Roles = "Admin,WarehouseManager,NightShiftWarehouseClerk,MorningShiftWarehouseClerk")]
        public async Task<IActionResult> WithdrawFromMain([FromBody] MainInventoryWithdrawaDto withdrawlDto)
        {
            var result = await _warehouseService.WithdrawFromMain(withdrawlDto);
            return StatusCode(result.StatusCode, result);
        }

    }
}
