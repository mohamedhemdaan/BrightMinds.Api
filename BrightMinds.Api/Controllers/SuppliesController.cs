using BrightMinds.Services.Dtos.SupplyDtos;
using BrightMinds.Services.Dtos.SupplyInstallmentDtos;
using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightMinds.Api.Controllers
{
    public class SuppliesController : ApiBaseController
    {

        private readonly ISupplyService _supplyService;
        
        public SuppliesController(ISupplyService supplyService)
        {
            _supplyService = supplyService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,WarehouseManager,NightShiftWarehouseClerk,MorningShiftWarehouseClerk")]
        public async Task<IActionResult> GetAll([FromQuery] SupplySpecParams specParams)
        {
            var result = await _supplyService.GetAllAsync(specParams);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,WarehouseManager,NightShiftWarehouseClerk,MorningShiftWarehouseClerk")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _supplyService.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,WarehouseManager,NightShiftWarehouseClerk,MorningShiftWarehouseClerk")]
        public async Task<IActionResult> Create([FromBody] SupplyDto supplyDto)
        {
            var result = await _supplyService.CreateAsync(supplyDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{supplyId}/Installments")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSupplyInstallment(int supplyId,[FromForm] SupplyInstallmentDto supplyInstallmentDto)
        {
            var result = await _supplyService.CreateSupplyInstallmentAsync(supplyId, supplyInstallmentDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{SupplyId}/Installments")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllSupplyInstallments(int SupplyId)
        {
            var speParams = new SupplyInstallmentSpecParams()
            {
                SupplyId = SupplyId
            };

            var result = await _supplyService.GetSupplyInstallmentsAsync(speParams);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("Installments/{installmentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetInstallment(int installmentId)
        {
            var result = await _supplyService.GetSupplyInstallmentByIdAsync(installmentId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("Installments")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllInstallments([FromQuery] SupplyInstallmentSpecParams specParams)
        {
            var result = await _supplyService.GetSupplyInstallmentsAsync(specParams);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("Installments/{installmentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSupplyInstallment(int installmentId, SupplyInstallmentDto supplyInstallmentDto)
        {
            var result = await _supplyService.UpdateSupplyInstallmentAsync(installmentId, supplyInstallmentDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("Installments/{installmentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSupplyInstallment(int installmentId)
        {
            var result = await _supplyService.DeleteSupplyInstallmentAsync(installmentId);
            return StatusCode(result.StatusCode, result);
        }

    }
}
