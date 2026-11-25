using BrightMinds.Services.Dtos.UnitDtos;
using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightMinds.Api.Controllers
{
    public class UnitsController : ApiBaseController
    {
        private readonly IUnitService _unitService;
        public UnitsController(IUnitService unitService)
        {
            _unitService = unitService;
        }

        [HttpPost("base-units")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBaseUnitAsync([FromForm] BaseUnitDto dto)
        {
            var result = await _unitService.BaseUnitService.CreateBaseUnitAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("complex-units")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateComplexUnitAsync([FromForm] ComplexUnitDto dto)
        {
            var result = await _unitService.ComplexUnitService.CreateComplexUnitAsync(dto);
            return StatusCode(result.StatusCode, result);
        }



        [HttpGet("base-units")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllBaseUnitsAsync([FromQuery] UnitSpecParams specParams)
        {
            var result = await _unitService.BaseUnitService.GetAll(specParams);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("complex-units")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllComplexUnitsAsync([FromQuery] UnitSpecParams specParams)
        {
            var result = await _unitService.ComplexUnitService.GetAll(specParams);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet("base-units/{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetBaseUnitAsync([FromRoute] int id)
        {
            var result = await _unitService.BaseUnitService.GetAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("complex-units/{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetComplexUnitAsync([FromRoute] int id)
        {
            var result = await _unitService.ComplexUnitService.GetAsync(id);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet("categories")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GatUnitCategoriessAsync()
        {
            var result = await _unitService.GetAllUnitCategories();
            return StatusCode(result.StatusCode, result);
        }




        [HttpPut("base-units/{id}")]
        public async Task<IActionResult> UpdateBaseUnit([FromRoute] int id, [FromForm] BaseUnitDto dto)
        {
            var result = await _unitService.BaseUnitService.UpdateBaseUnitAsync(id, dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("complex-units/{id}")]
        public async Task<IActionResult> UpdateComplexUnit([FromRoute] int id, [FromForm] ComplexUnitDto dto)
        {
            var result = await _unitService.ComplexUnitService.UpdateComplexUnitAsync(id, dto);
            return StatusCode(result.StatusCode, result);
        }




        [HttpDelete("base-Units/{id}")]
        public async Task<IActionResult> DeleteBaseUnit(int id)
        {
            var result = await _unitService.BaseUnitService.DeleteBaseUnitAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("complex-Units/{id}")]
        public async Task<IActionResult> DeleteComplexUnit(int id)
        {
            var result = await _unitService.ComplexUnitService.DeleteComplexUnitAsync(id);
            return StatusCode(result.StatusCode, result);
        }

    }
}
