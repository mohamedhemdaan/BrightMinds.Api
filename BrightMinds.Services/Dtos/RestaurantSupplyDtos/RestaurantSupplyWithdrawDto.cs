using System.ComponentModel.DataAnnotations;

namespace BrightMinds.Services.Dtos.RestaurantSupplyDtos
{
    public class RestaurantSupplyWithdrawDto
    {
        [Required]
        public int RawMaterialId { get; set; }

        [Required]

        public string WithdrawnBy { get; set; }

        [Required]
        [Range(0.01, 999999999, ErrorMessage = "الكمية يجب أن تكون أكبر من 0")]
        public decimal Quantity { get; set; }         

        public string? Notes { get; set; }

        public bool IsWithdrawByWeight { get; set; } = false;       
    }
}