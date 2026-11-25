using BrightMinds.Core.ISpecificationParams;
using BrightMinds.Services.Dtos.RestaurantSupplyDtos;
using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightMinds.Api.Controllers
{
    [Authorize(Roles = "Admin,MorningShiftWarehouseClerk,NightShiftWarehouseClerk,WarehouseManager")]
    public class RestaurantSupplyController : ApiBaseController
    {
        private readonly IRestaurantSupplyService _restaurantSupplyService;

        public RestaurantSupplyController(IRestaurantSupplyService restaurantSupplyService)
        {
            _restaurantSupplyService = restaurantSupplyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationSpecParams specParams)
        {
            var result = await _restaurantSupplyService.GetAllSuppliesAsync(specParams);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _restaurantSupplyService.GetSupplyByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] RestaurantSupplyWithdrawDto dto)
        {
            var result = await _restaurantSupplyService.WithdrawAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}/withdraw-history")]
        public async Task<IActionResult> GetWithdrawHistory(int id)
        {
            var result = await _restaurantSupplyService.GetWithdrawHistoryAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("withdraw-history")]
        public async Task<IActionResult> GetAllWithdrawHistory([FromQuery] PaginationSpecParams specParams)
        {
            var result = await _restaurantSupplyService.GetAllWithdrawHistoryAsync(specParams);
            return StatusCode(result.StatusCode, result);
        }
    }
}