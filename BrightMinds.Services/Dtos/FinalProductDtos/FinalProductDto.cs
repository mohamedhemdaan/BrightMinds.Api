using BrightMinds.Core.Enums;
using BrightMinds.Services.Dtos.FinalProductIngredientDtos;
using System.ComponentModel.DataAnnotations;

namespace BrightMinds.Services.Dtos.FinalProductDtos
{
    public class FinalProductDto
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        [GreaterThanZero(ErrorMessage = "سعر البيع لازم يكون اكبر من الصفر")]
        public decimal SellingPrice { get; set; }

        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }   

        [MinLength(1, ErrorMessage = "لازم يكون فيه مكون واحد على الاقل")]
        public List<FinalProductIngredientDto> Ingredients { get; set; }
    }

    public class ConsumedItem
    {
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public UnitType UnitType { get; set; }
        public decimal Price { get; set; }
        public FinalProductIngredientType Type { get; set; }
    }
    public class GreaterThanZeroAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is decimal d)
                return d > 0;

            return false;
        }
    }
}
