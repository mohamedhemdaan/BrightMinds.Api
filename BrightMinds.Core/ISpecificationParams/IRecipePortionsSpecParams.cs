
namespace BrightMinds.Core.ISpecificationParams
{
    public interface IRecipePortionsSpecParams
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string? RecipeName { get; set; }
        public string? OrderBy { get; set; }
    }
}
