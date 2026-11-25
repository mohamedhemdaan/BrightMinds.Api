
using BrightMinds.Core.Enums;

namespace BrightMinds.Core.ISpecificationParams
{
    public interface ISupplyReturnSpecParams
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public SupplyReturnStatus? Status { get; set; }
        public string? OrderBy { get; set; }
    }
}
