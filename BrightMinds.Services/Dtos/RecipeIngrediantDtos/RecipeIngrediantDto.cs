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
        [Range(0.000001, double.MaxValue)]
        public decimal Quantity { get; set; }
        [Required]
        public UnitType UnitType { get; set; }

        /// <summary>Optional for backward compatibility. When supplied for a raw material, identifies its purchase/middle/smallest unit.</summary>
        public RawMaterialUnitLevel? RawMaterialUnitLevel { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IngredientType == IngredientType.SubRecipe && SubRecipeId == null)
                yield return new ValidationResult("SubRecipe ID is required when ingredient type is sub recipe!", new[] { nameof(SubRecipeId) });
            else if (IngredientType == IngredientType.RawMaterial && RawMaterialId == null)
                yield return new ValidationResult("Raw Material ID is required when ingredient type is Raw Material!", new[] { nameof(RawMaterialId) });
            else if ((RawMaterialId == null && SubRecipeId == null) || (RawMaterialId != null && SubRecipeId != null))
                yield return new ValidationResult("The recipe ingredient must be either Raw Material Or Sub Recipe!", new[] { nameof(RawMaterialId), nameof(SubRecipeId) });
            else if (IngredientType == IngredientType.SubRecipe && RawMaterialUnitLevel.HasValue)
                yield return new ValidationResult("Unit level can only be set for raw-material ingredients.", new[] { nameof(RawMaterialUnitLevel) });
        }
    }
}
