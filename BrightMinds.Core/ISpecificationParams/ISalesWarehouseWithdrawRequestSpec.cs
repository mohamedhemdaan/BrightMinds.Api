using BrightMinds.Core.Enums;

namespace BrightMinds.Core.ISpecificationParams
{
    public interface ISalesWarehouseWithdrawRequestSpec
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string? OrderBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public RequestStatus? Status { get; set; }

        public int? CategoryId { get; set; }      

    }
}
