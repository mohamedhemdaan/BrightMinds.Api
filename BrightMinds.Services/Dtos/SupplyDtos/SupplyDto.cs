
using BrightMinds.Services.Dtos.SupplyItemDtos;
using System.ComponentModel.DataAnnotations;

namespace BrightMinds.Services.Dtos.SupplyDtos
{
    public class SupplyDto
    {
        [Required]
        public string RecipientName { get; set; }
        [Required]
        public int SupplierId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "مينفعش تدخل المدفوعات بالسالب")]
        public decimal PaidAmount { get; set; } = 0;

        [Required]
        public List<SupplyItemDto> SupplyItems { get; set; }
    }
}
