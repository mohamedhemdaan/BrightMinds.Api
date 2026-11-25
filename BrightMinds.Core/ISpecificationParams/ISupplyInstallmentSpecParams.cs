
namespace BrightMinds.Core.ISpecificationParams
{
    public interface ISupplyInstallmentSpecParams
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int? SupplyId { get; set; }
        public DateTime? Date { get; set; }
        public string? OrderBy { get; set; }
    }
}
