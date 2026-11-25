using BrightMinds.Services.Dtos.FinalProductSubCategoryDtos;
using BrightMinds.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightMinds.Api.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Authorize]
    public class FinalProductSubCategoriesController : ApiBaseController
    {
        private readonly IFinalProductSubCategoryService _subCategoryService;

        public FinalProductSubCategoriesController(IFinalProductSubCategoryService subCategoryService)
        {
            _subCategoryService = subCategoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _subCategoryService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("by-category/{categoryId}")]
        public async Task<IActionResult> GetByCategoryId(int categoryId)
        {
            var result = await _subCategoryService.GetByCategoryIdAsync(categoryId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFinalProductSubCategoryDto dto)
        {
            var result = await _subCategoryService.CreateAsync(dto.Name, dto.CategoryId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _subCategoryService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}