using System.ComponentModel.DataAnnotations.Schema;

namespace BrightMinds.Core.Models
{
    public class BaseUnit : Unit
    {
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public UomCategory Category { get; set; }
        public float? ConversionFactor { get; set; } // nullable, because the custom unit types does'nt have a conversion factor
    }
}
