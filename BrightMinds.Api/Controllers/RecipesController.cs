using BrightMinds.Services.Dtos.RecipeDtos;
using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightMinds.Api.Controllers
{

    public class RecipesController : ApiBaseController
    {
        private readonly IRecipeService _recipeService;
        public RecipesController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,OperationChef")]
        public async Task<IActionResult> GetAll([FromQuery] RecipeSpecParams specParams)
        {
            var result = await _recipeService.GetAllAsync(specParams);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,OperationChef")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _recipeService.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody]RecipeDto recipeDto)
        {
            var result = await _recipeService.CreateAsync(recipeDto);
            return StatusCode(result.StatusCode,  result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody]RecipeDto recipeDto, int id)
        {
            var result = await _recipeService.UpdateAsync(recipeDto, id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _recipeService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
