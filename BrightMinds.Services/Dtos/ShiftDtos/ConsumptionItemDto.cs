using System.ComponentModel.DataAnnotations;

namespace BrightMinds.Services.Dtos.ShiftDtos
{
    public class ConsumptionItemDto
    {
        [Required]
        public int WithdrawItemId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal ConsumedQuantity { get; set; }
    }
}