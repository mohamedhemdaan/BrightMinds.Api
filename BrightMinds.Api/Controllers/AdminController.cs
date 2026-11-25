using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightMinds.Api.Controllers
{
    public class AdminController : ApiBaseController
    {
        private readonly IWarehouseService _warehouseService;

        public AdminController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpGet("Withdraw-History")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetWithdrawHistory([FromQuery] WithdrawHistorySpecParams specParams)
        {
            var result = await _warehouseService.GetWithrawHistory(specParams);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("Warehouses")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllWarehouses()
        {
            var result = await _warehouseService.GetAllWarehouses();
            return StatusCode(result.StatusCode, result);
        }

    }
}
