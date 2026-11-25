
using System.ComponentModel.DataAnnotations.Schema;

namespace BrightMinds.Core.Models
{
    public class Recipe : BaseEntity
    {
        public string Name { get; set; } 
        public string Category { get; set; }
        public decimal? PortianSizeInGrams { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalSize { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal NormalLossSize { get; set; }
        public decimal TotalCost { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal CostPerKilo {  get; set; }
        public decimal? ExpectedPortiansnumber { get; set; }
        public bool IsFlexible { get; set; }
        public bool IsSubRecipe { get; set; }
        public bool IsDeleted { get; set; } = false;

        public bool RequiresSettlement { get; set; } = false;    

        public int? PortionCategoryId { get; set; }
        [ForeignKey(nameof(PortionCategoryId))]
        public PortionCategory PortionCategory { get; set; }
        public virtual ICollection<RecipeIngrediant> Ingrediants { get; set;} = new List<RecipeIngrediant>();
        public virtual ICollection<RecipePortion> Portions { get; set;} = new List<RecipePortion>();
    }
}
