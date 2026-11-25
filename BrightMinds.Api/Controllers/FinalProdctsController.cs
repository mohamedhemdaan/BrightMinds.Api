using BrightMinds.Services.Dtos.FinalProductDtos;
using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightMinds.Api.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Authorize]
    public class FinalProdctsController : ApiBaseController
    {
        private readonly IFinalProductService _finalProductService;

        public FinalProdctsController(IFinalProductService finalProductService)
        {
            _finalProductService = finalProductService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] FinalProductDto finalProduct)
        {
            var result = await _finalProductService.CreateFinalProduct(finalProduct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _finalProductService.GetFinalProductById(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] FinalProductSpecParams speParams)
        {
            var result = await _finalProductService.GetAllFinalProducts(speParams);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromBody] FinalProductDto finalProduct, int id)
        {
            var result = await _finalProductService.UpdateFinalProduct(finalProduct, id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _finalProductService.DeleteFinalProduct(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
