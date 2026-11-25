using System.ComponentModel.DataAnnotations;

namespace BrightMinds.Services.Dtos.SupplyInstallmentDtos
{
    public class SupplyInstallmentDto
    {
        [Required]
        public int SupplyId { get; set; }
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "لازم المبلغ المدفوع يكون اكبر من 1")]
        public decimal AmountPaid { get; set; }
        [Required]
        public DateTime PaymentDate { get; set; }
    }
}
