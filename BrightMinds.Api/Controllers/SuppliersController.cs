using BrightMinds.Services.Dtos.SupplierDtos;
using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightMinds.Api.Controllers
{
    [Authorize(Roles = "Admin,WarehouseManager,NightShiftWarehouseClerk,MorningShiftWarehouseClerk")]
    public class SuppliersController : ApiBaseController
    {
        private readonly ISupplierService _supplierService;
        
        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SupplierSpecParams specParams)
        {
            var result = await _supplierService.GetAllAsync(specParams);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _supplierService.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] SupplierDto supplierDto)
        {
            var result = await _supplierService.CreateAsync(supplierDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromForm] SupplierDto supplierDto, int id)
        {
            var result = await _supplierService.UpdateAsync(supplierDto, id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _supplierService.DleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
