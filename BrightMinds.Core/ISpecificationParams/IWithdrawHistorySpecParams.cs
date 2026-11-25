using BrightMinds.Core.Enums;

namespace BrightMinds.Core.ISpecificationParams
{
    public interface IWithdrawHistorySpecParams
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? RecipientSearchName { get; set; }
        public string? OrderBy { get; set; }
        public int? WarehouseId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int? ShiftId { get; set; }
    }
}