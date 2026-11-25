
using BrightMinds.Core.Enums;
using BrightMinds.Services.Dtos.SupplyInstallmentDtos;
using BrightMinds.Services.Dtos.SupplyItemDtos;

namespace BrightMinds.Services.Dtos.SupplyDtos
{
    public class SupplyDetailsDto
    {
        public int Id { get; set; }
        public string RecipientName { get; set; }
        public DateTime CreationDate { get; set; }
        public decimal TotalCost { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal RefundAmount { get; set; }
        public SupplyPaymentStatus Status { get; set; }
        public int SupplierId { get; set; }
        public List<SupplyItemDetailsDto> SupplyItems { get; set; }
        public List<SupplyInstallmentDetailsDto> Installments { get; set; }

    }
}
