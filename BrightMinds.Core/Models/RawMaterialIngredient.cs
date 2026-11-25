using System.ComponentModel.DataAnnotations.Schema;

namespace BrightMinds.Core.Models
{
    public class RawMaterialIngredient : FinalProductIngredient
    {
        [ForeignKey("RawMaterial")]
        public int RawMaterialId { get; set; }
        public RawMaterial RawMaterial { get; set; }
    }
}
