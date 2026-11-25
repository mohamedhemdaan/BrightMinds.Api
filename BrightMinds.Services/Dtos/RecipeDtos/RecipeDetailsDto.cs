using BrightMinds.Services.Dtos.RecipeIngrediantDtos;

namespace BrightMinds.Services.Dtos.RecipeDtos
{
    public class RecipeDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal? PortianSizeInGrams { get; set; }
        public decimal TotalSize { get; set; }
        public decimal TotalCost { get; set; }
        public decimal? ExpectedPortiansnumber { get; set; }
        public bool IsFlexible { get; set; }
        public bool IsSubRecipe { get; set; }

        public int? PortionCategoryId { get; set; }
        public string? PortionCategoryName { get; set; }

        public bool RequiresSettlement { get; set; }

        public decimal NormalLossSize { get; set; }  

        public List<RecipeIngrediantDetailsDto> Ingrediants { get; set; }
    }
}
