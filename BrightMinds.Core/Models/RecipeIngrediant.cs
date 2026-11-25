using BrightMinds.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrightMinds.Core.Models
{
    public class RecipeIngrediant : BaseEntity
    {
        [ForeignKey("Recipe")] 
        public int RecipeId { get; set; }
        public virtual Recipe Recipe { get; set; }

        [ForeignKey("RawMaterial")]
        public int? RawMaterialId { get; set; }
        public virtual RawMaterial RawMaterial { get; set; }

        [ForeignKey("SubRecipe")]
        public int? SubRecipeId { get; set; }
        public Recipe SubRecipe { get; set; }

        public IngredientType IngredientType { get; set; }

        public decimal Quantity { get; set; }
        public UnitType UnitType { get; set; }
    }

    public static class RecipeIngrediantExtensions
    {
        public static bool IsValid(this RecipeIngrediant recipeIngredient, out string error)
        {
            if (recipeIngredient.IngredientType == IngredientType.SubRecipe && recipeIngredient.SubRecipeId == null)
            {
                error = "SubRecipe ID is required when ingredient type is sub recipe!";
                return false;
            }
            else if(recipeIngredient.IngredientType == IngredientType.RawMaterial && recipeIngredient.RawMaterialId == null)
            {
                error = "Raw Material ID is required when ingredient type is Raw Material!";
                return false;
            }
            else if((recipeIngredient.RawMaterialId == null && recipeIngredient.SubRecipeId == null)||
                    (recipeIngredient.RawMaterialId != null && recipeIngredient.SubRecipeId != null))
            {
                error = "The recipie ingredient must me either Raw Material Or Sub Recipe!";
                return false;
            }

            error = null;
            return true;
        }
    }
}
