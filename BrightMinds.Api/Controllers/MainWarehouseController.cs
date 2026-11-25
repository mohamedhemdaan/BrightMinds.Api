    using BrightMinds.Core.Enums;
using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightMinds.Api.Controllers
{
    //[Authorize(Roles = "Admin,MorningShiftWarehouseClerk,NightShiftWarehouseClerk,WarehouseManager")]
    public class MainWarehouseController : ApiBaseController
    {
        private readonly IWarehouseService _WarehouseService;
        public MainWarehouseController(IWarehouseService warehouseService)
        {
            _WarehouseService = warehouseService;
        }

        [HttpGet("Inventory")]
        public async Task<IActionResult> GetInventory([FromQuery] WarehouseInventorySpecParams specParams)
        {
            var result = await _WarehouseService.GetAllSuppliesInventoryAsync(specParams,WarehouseType.MainWarehouse);
            return StatusCode(result.StatusCode, result);
        }
    }
}
