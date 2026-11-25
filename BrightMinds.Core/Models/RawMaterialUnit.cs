using System.ComponentModel.DataAnnotations.Schema;

namespace BrightMinds.Core.Models
{
    public class RawMaterialUnit : BaseEntity
    {
        [ForeignKey("RawMaterial")]
        public int RawMaterialId { get; set; }
        public RawMaterial RawMaterial { get; set; }


        [ForeignKey("Unit")]
        public int UnitId { get; set; }
        public Unit Unit { get; set; }

        public float ConversionFactor { get; set; }
    }
}
