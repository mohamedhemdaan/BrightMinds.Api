
namespace BrightMinds.Core.ISpecificationParams
{
    public interface IRecipeSpecParams
    {  
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? SearchName { get; set; }
        public string? OrderBy { get; set; }

        public string? Category { get; set; }      

    }
}
