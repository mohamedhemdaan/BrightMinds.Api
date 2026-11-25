using BrightMinds.Services.Dtos.RecipeDtos;
using BrightMinds.Services.Dtos.RecipePortionDtos;
using BrightMinds.Services.SpecificationParams;

namespace BrightMinds.Services.IServices
{
    public interface IRecipeService
    {
        public Task<Response> GetAllAsync(RecipeSpecParams specParams);
        public Task<Response> GetByIdAsync(int id);
        public Task<Response> CreateAsync(RecipeDto recipeDto);
        public Task<Response> UpdateAsync(RecipeDto recipeDto, int id);
        public Task<Response> DeleteAsync(int id);
        public Task<Response> AddRecipePortions(RecipePortionDto recipePortions);
        public Task<Response> GetAllRecipesPortions(RecipePortionsSpecParams specParams);
        public Task<Response> GetRecipePortionById(int id);
        public Task<Response> UpdateRecipePortion(RecipePortionDto recipePortion);
        public Task<Response> DeleteRecipePortion(int id);
        public Task<Response> DeleteAllRecipePortions(int recipeId);
    }
}
