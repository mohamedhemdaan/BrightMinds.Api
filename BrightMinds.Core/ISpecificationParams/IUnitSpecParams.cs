
namespace BrightMinds.Core.ISpecificationParams
{
    public interface IUnitSpecParams
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? SearchName { get; set; }
        public string? OrderBy { get; set; }

    }
}
