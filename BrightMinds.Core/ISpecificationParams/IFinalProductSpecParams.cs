
namespace BrightMinds.Core.ISpecificationParams
{
    public interface IFinalProductSpecParams
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string? SearchName { get; set; }
        public string? OrderBy { get; set; }
        public int? CategoryId { get; set; }      

        public int? SubCategoryId { get; set; }
    }
}
