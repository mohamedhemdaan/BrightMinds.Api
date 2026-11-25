
using System.ComponentModel.DataAnnotations.Schema;

namespace BrightMinds.Core.Models
{
    public class RecipePortion : BaseEntity
    {
        public string Name { get; set; }
        public decimal SizeInGrams { get; set; }

        [ForeignKey("Recipe")]
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }

        public int? CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public PortionCategory Category { get; set; }
    }
}
