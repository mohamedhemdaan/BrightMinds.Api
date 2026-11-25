using BrightMinds.Core.Interfaces;
using BrightMinds.Services.Dtos.RawMaterialDtos;
using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightMinds.Api.Controllers
{
    [Authorize(Roles = "Admin,MorningShiftWarehouseClerk,NightShiftWarehouseClerk,WarehouseManager")]
    public class RawMaterialsController : ApiBaseController
    {
        private readonly IRawMaterialService _rawMaterialService;

        public RawMaterialsController(IRawMaterialService rawMaterialService, IUnitOfWork unitOfWork)
        {
            _rawMaterialService = rawMaterialService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] RawMaterialSpecParams specParams)
        {
            var result = await _rawMaterialService.GetAllAsync(specParams);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _rawMaterialService.GetByIdAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] RawMaterialDto rawMaterialDto)
        {
            var result = await _rawMaterialService.CreateAsync(rawMaterialDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromForm] RawMaterialDto rawMaterialDto, int id)
        {
            var result = await _rawMaterialService.UpdateAsync(rawMaterialDto, id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _rawMaterialService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }

    }
}
