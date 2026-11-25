using BrightMinds.Services.Dtos.ShiftDtos;
using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightMinds.Api.Controllers
{
    public class ShiftsController : ApiBaseController
    {
        private readonly IShiftService _shiftService;
        
        public ShiftsController(IShiftService shiftService)
        {
            _shiftService = shiftService;
        }

        [HttpPost("Start")]
        [Authorize]
        public async Task<IActionResult> CreateAsync()
        {
            var result = await _shiftService.CreateShift();
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("Close")]
        [Authorize]
        public async Task<IActionResult> CloseAsync()
        {
            var result = await _shiftService.CloseShift();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("Latest-Shift")]
        [Authorize]
        public async Task<IActionResult> GetLatestShiftAsync()
        {
            var result = await _shiftService.GetLastestShift();
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{id}/ForceClose")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ForceCloseAsync(int id)
        {
            var result = await _shiftService.ForceCloseShift(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _shiftService.GetShiftById(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAsync([FromQuery] ShiftSpecParams specParams)
        {
            var result = await _shiftService.GetAllShifts(specParams);
            return StatusCode(result.StatusCode, result);
        }


        [HttpPost("{shiftId}/Consumption")]
        [Authorize]
        public async Task<IActionResult> RecordConsumption(int shiftId, [FromBody] List<ConsumptionItemDto> consumptionList)
        {
            var result = await _shiftService.RecordConsumption(shiftId, consumptionList);
            return StatusCode(result.StatusCode, result);
        }




    }
}
