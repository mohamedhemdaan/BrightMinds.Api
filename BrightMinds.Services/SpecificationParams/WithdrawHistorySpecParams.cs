using BrightMinds.Core.Enums;
using BrightMinds.Core.ISpecificationParams;

namespace BrightMinds.Services.SpecificationParams
{
    public class WithdrawHistorySpecParams : IWithdrawHistorySpecParams
    {
        private const int MaxPageSize = 15;
        private int _pageSize = 5;
        public int PageIndex { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        public string? RecipientSearchName { get; set; }
        public string? OrderBy { get; set; }
        public int? WarehouseId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int? ShiftId { get; set; }
    }
}