using BrightMinds.Core.ISpecificationParams;

namespace BrightMinds.Services.SpecificationParams
{
    public class FinalProductSpecParams : IFinalProductSpecParams
    {
        private const int MaxPageSize = 15;      
        private int _pageSize = 5;       
        public int PageIndex { get; set; } = 1;   

        public int PageSize
        {
            get => _pageSize;
            set
            {
                _pageSize = value > MaxPageSize ? MaxPageSize : value;
            }
        }

        public string? SearchName { get; set; }
        public string? OrderBy { get; set; }

        public int? CategoryId { get; set; }      
        public int? SubCategoryId { get; set; }
    }
}
