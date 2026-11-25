
using System.ComponentModel.DataAnnotations.Schema;

namespace BrightMinds.Core.Models
{
    public class FinalProduct : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? SellingPrice { get; set; }

        public int? CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public FinalProductCategory Category { get; set; }

        public int? SubCategoryId { get; set; }
        [ForeignKey(nameof(SubCategoryId))]
        public FinalProductSubCategory SubCategory { get; set; }
        public virtual List<FinalProductIngredient> Ingredients { get; set; } = new List<FinalProductIngredient>();
    }
}
