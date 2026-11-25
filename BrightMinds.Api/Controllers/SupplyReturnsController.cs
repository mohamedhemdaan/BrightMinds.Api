using BrightMinds.Services.Dtos.SupplyReturnDtos;
using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightMinds.Api.Controllers
{
    [ApiController]
    public class SupplyReturnsController : ApiBaseController
    {
        private readonly ISupplyReturnService _supplyReturnService;

        public SupplyReturnsController(ISupplyReturnService supplyReturnService)
        {
            _supplyReturnService = supplyReturnService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,MorningShiftWarehouseClerk,NightShiftWarehouseClerk,WarehouseManager")]
        public async Task<IActionResult> CreateAsync([FromBody] SupplyReturnDto dto)
        {
            var result = await _supplyReturnService.CreateSupplyReturn(dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,MorningShiftWarehouseClerk,NightShiftWarehouseClerk,WarehouseManager")]
        public async Task<IActionResult> GetAllAsync([FromQuery] SupplyReturnSpecParams specParams)
        {
            var result = await _supplyReturnService.GetAllSupplyReturns(specParams);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,MorningShiftWarehouseClerk,NightShiftWarehouseClerk,WarehouseManager")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _supplyReturnService.GetSupplyReturnById(id);
            return StatusCode(result.StatusCode, result);
        }



        [HttpGet("Supply/{supplyId}")]
        [Authorize(Roles = "Admin,MorningShiftWarehouseClerk,NightShiftWarehouseClerk,WarehouseManager")]

        public async Task<IActionResult> GetAllWithSupplyAsync(int supplyId)
        {
            var result = await _supplyReturnService.GetAllSupplyReturnsForSupply(supplyId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{id}/Approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveSupplyReturnAsync(int id)
        {
            var result = await _supplyReturnService.ApproveSupplyReturn(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{id}/Reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectSupplyReturnAsync(int id)
        {
            var result = await _supplyReturnService.RejectSupplyReturn(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
