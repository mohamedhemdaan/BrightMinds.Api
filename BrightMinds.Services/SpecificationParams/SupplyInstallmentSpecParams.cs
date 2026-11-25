using BrightMinds.Core.ISpecificationParams;

namespace BrightMinds.Services.SpecificationParams
{
    public class SupplyInstallmentSpecParams : ISupplyInstallmentSpecParams
    {
        private const int MaxPageSize = 15; //The maximum Size of the page
        private int _pageSize = 5; //The page size is 5 by default
        public int PageIndex { get; set; } = 1; //be default one


        public int PageSize
        {
            get => _pageSize;
            set
            {
                _pageSize = value > MaxPageSize ? MaxPageSize : value;
            }
        }


        public int? SupplyId { get; set; }
        public DateTime? Date { get; set; }
        public string? OrderBy { get; set; }

    }
}
