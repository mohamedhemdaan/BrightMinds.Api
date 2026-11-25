using BrightMinds.Services.Dtos.RecipeIngrediantDtos;
using System.ComponentModel.DataAnnotations;

namespace BrightMinds.Services.Dtos.RecipeDtos
{
    public class RecipeDto : IValidatableObject
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Category { get; set; }
        public decimal NormalLossSize { get; set; } = default;
        public decimal? PortianSizeInGrams { get; set; }

        [Required]
        public decimal TotalSize { get; set; }
        public decimal? ExpectedPortiansnumber { get; set; }
        [Required]
        public bool IsFlexible { get; set; }
        [Required]
        public bool IsSubRecipe { get; set; }

        [MinLength(1, ErrorMessage = "لازم تحدد مكون واحد على الاقل")]
        public  List<RecipeIngrediantDto> Ingrediants { get; set; }

        public int? PortionCategoryId { get; set; }         

        public bool RequiresSettlement { get; set; } = false;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IsFlexible == true && PortianSizeInGrams != null)
                yield return new ValidationResult("مينفعش تحدد حجم البرسيلة", new[] { "PortianSizeInGrams" });

            if(IsFlexible == false && PortianSizeInGrams == null)
                yield return new ValidationResult("لازم تحدد حجم البرسيلة", new[] { "PortianSizeInGrams" });

            if (IsFlexible == true && ExpectedPortiansnumber != null)
                yield return new ValidationResult("مينفعش تحدد عدد البرسيلات", new[] { "PortianSizeInGrams" });

            if(IsFlexible == false && ExpectedPortiansnumber == null)
                yield return new ValidationResult("لازم تحدد عدد البرسيلات", new[] { "PortianSizeInGrams" });
        }
    }
}
