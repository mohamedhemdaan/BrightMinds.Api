using BrightMinds.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrightMinds.Core.Models
{
    public class SupplyReturnItem : BaseEntity
    {
        [ForeignKey("SupplyReturn")]
        public int SupplyReturnId { get; set; }
        public SupplyReturn SupplyReturn { get; set; }

        [ForeignKey("SupplyItem")]
        public int SupplyItemId { get; set; }
        public SupplyItem SupplyItem { get; set; }

        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public UnitType UnitType { get; set; }
        public decimal Price { get; set; }
    }
}
