using BrightMinds.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightMinds.Api.Controllers
{
    //[Authorize(Roles = "Admin,OperationChef")]
    [Authorize]
    public class RecipeCategoriesController : ApiBaseController
    {
        private readonly IRecipeCategoryService _recipeCategoryService;

        public RecipeCategoriesController(IRecipeCategoryService recipeCategoryService)
        {
            _recipeCategoryService = recipeCategoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _recipeCategoryService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string name)
        {
            var result = await _recipeCategoryService.CreateAsync(name);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _recipeCategoryService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}