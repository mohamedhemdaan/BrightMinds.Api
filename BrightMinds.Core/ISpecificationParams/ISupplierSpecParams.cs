
namespace BrightMinds.Core.ISpecificationParams
{
    public interface ISupplierSpecParams
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? OrderBy { get; set; }
        public string? SearchName { get; set; }

    }
}
