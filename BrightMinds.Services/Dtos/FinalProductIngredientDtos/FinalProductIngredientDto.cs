using BrightMinds.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace BrightMinds.Services.Dtos.FinalProductIngredientDtos
{
    public class FinalProductIngredientDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public decimal Quantity { get; set; }
        [Required]
        public UnitType UnitType { get; set; }
        [Required]
        public FinalProductIngredientType Type { get; set; }

    }
}
