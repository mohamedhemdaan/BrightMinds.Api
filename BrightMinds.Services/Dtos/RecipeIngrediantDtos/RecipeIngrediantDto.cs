using BrightMinds.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace BrightMinds.Services.Dtos.RecipeIngrediantDtos
{
    public class RecipeIngrediantDto : IValidatableObject
    {
        public int? RawMaterialId { get; set; }
        public int? SubRecipeId { get; set; }
        [Required]
        public IngredientType IngredientType { get; set; }
        [Required]
        public decimal Quantity { get; set; }
        [Required]
        public UnitType UnitType { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IngredientType == IngredientType.SubRecipe && SubRecipeId == null)
            {
                yield return new ValidationResult("SubRecipe ID is required when ingredient type is sub recipe!", new[] {nameof(SubRecipeId)});
            }
            else if (IngredientType == IngredientType.RawMaterial && RawMaterialId == null)
            {
                yield return new ValidationResult("Raw Material ID is required when ingredient type is Raw Material!", new[] { nameof(SubRecipeId) });

            }
            else if ((RawMaterialId == null && SubRecipeId == null) ||
                    (RawMaterialId != null && SubRecipeId != null))
            {
                yield return new ValidationResult("The recipie ingredient must me either Raw Material Or Sub Recipe!", new[] { nameof(SubRecipeId) });
            }
        }
    }

}
