using System.ComponentModel.DataAnnotations.Schema;

namespace BrightMinds.Core.Models
{
    public class PortionIngredient : FinalProductIngredient
    {
        [ForeignKey("ProductPortion")]
        public int ProductPortionId { get; set; }
        public RecipePortion ProductPortion { get; set; }
    }
}
