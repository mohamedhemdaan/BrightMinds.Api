using System.ComponentModel.DataAnnotations.Schema;

namespace BrightMinds.Core.Models
{
    public class RestaurantSupplyWithdraw : BaseEntity
    {
        [ForeignKey("RawMaterial")]
        public int RawMaterialId { get; set; }
        public RawMaterial RawMaterial { get; set; }
        public string WithdrawnBy { get; set; }
        public decimal Quantity { get; set; }    
        public DateTime WithdrawDate { get; set; }
        public string? Notes { get; set; }

        public bool IsWithdrawnByWeight { get; set; } = false;       
        public decimal? WithdrawnWeight { get; set; }    
    }
}