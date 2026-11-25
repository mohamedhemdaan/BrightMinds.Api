using BrightMinds.Core.Enums;

namespace BrightMinds.Core.ISpecificationParams
{
    public interface ISupplySpecParams
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? OrderBy { get; set; }
        public string? RecipientName { get; set; }

        public SupplyPaymentStatus? PaymentStatus { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}