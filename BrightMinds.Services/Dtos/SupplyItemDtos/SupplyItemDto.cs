using System.ComponentModel.DataAnnotations;
using BrightMinds.Core.Enums;

namespace BrightMinds.Services.Dtos.SupplyItemDtos
{
    public class SupplyItemDto
    {
        [Required]
        public int RawMaterialId { get; set; }

        [Required(ErrorMessage = "طريقة الاستلام مطلوبة")]
        public ReceivingMethod ReceivedBy { get; set; } = ReceivingMethod.Container;

        public decimal? ContainerQuantity { get; set; }    
        public decimal? ContainerPrice { get; set; }       

        public decimal? ReceivedQuantity { get; set; }      
        public decimal? UnitPrice { get; set; }             

        public decimal? ReceivedWeightKg { get; set; }
        public decimal? ExchangePrice { get; set; }
        public decimal? PriceMultiplier { get; set; }
    }
}