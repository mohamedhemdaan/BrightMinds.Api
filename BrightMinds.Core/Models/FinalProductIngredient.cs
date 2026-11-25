using BrightMinds.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrightMinds.Core.Models
{
    public class FinalProductIngredient : BaseEntity
    {
        [ForeignKey("FinalProduct")]
        public int FinalProductId { get; set; }
        public FinalProduct FinalProduct { get; set; }

        public decimal Quantity { get; set; }
        public UnitType UnitType { get; set; }
        public FinalProductIngredientType Type { get; set; }
    }
}
