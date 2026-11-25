using BrightMinds.Core.Models;
using BrightMinds.Services.Dtos.RecipePortionDtos;
using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightMinds.Api.Controllers
{
    [Authorize]
    public class RecipPortionsController : ApiBaseController
    {
        private readonly IRecipeService _recipeService;
        public RecipPortionsController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]RecipePortionDto recipePortions)
        {
            var result = await _recipeService.AddRecipePortions(recipePortions);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery]RecipePortionsSpecParams specParams)
        {
            var result = await _recipeService.GetAllRecipesPortions(specParams);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _recipeService.GetRecipePortionById(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] RecipePortionDto recipePortion)
        {
            var result = await _recipeService.UpdateRecipePortion(recipePortion);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipePortion(int id)
        {
            var result = await _recipeService.DeleteRecipePortion(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("Recipes/{id}")]
        public async Task<IActionResult> DeleteAllPortions(int id)
        {
            var result = await _recipeService.DeleteAllRecipePortions(id);
            return StatusCode(result.StatusCode, result);
        }


    }
}
