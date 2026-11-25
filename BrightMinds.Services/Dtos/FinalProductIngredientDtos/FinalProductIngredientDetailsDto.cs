using BrightMinds.Core.Enums;

namespace BrightMinds.Services.Dtos.FinalProductIngredientDtos
{
    public class FinalProductIngredientDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public UnitType UnitType { get; set; }
        public FinalProductIngredientType Type { get; set; }
    }
}
