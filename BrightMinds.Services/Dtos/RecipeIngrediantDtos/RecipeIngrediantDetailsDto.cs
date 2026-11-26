using BrightMinds.Core.Enums;

namespace BrightMinds.Services.Dtos.RecipeIngrediantDtos
{
    public class RecipeIngrediantDetailsDto
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public int? RawMaterialId { get; set; }
        public int? SubRecipeId { get; set; }
        public string? RawMaterialName { get; set; }
        public string? SubRecipeName { get; set; }
        public decimal Quantity { get; set; }
        public UnitType UnitType { get; set; }
        public RawMaterialUnitLevel? RawMaterialUnitLevel { get; set; }
        public string? UnitName { get; set; }
        public decimal Price { get; set; }
    }
}
