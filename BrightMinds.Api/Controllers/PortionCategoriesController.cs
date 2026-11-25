using BrightMinds.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightMinds.Api.Controllers
{
    //[Authorize(Roles = "Admin,OperationChef")]
    [Authorize]
    public class PortionCategoriesController : ApiBaseController
    {
        private readonly IPortionCategoryService _portionCategoryService;

        public PortionCategoriesController(IPortionCategoryService portionCategoryService)
        {
            _portionCategoryService = portionCategoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _portionCategoryService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string name)
        {
            var result = await _portionCategoryService.CreateAsync(name);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _portionCategoryService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}