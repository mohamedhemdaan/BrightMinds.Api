using BrightMinds.Core.ISpecificationParams;
using BrightMinds.Core.Models;
using System.Linq.Expressions;

namespace BrightMinds.Core.Specifications
{
    public class FinalProductSpecifications : BaseSpecification<FinalProduct>
    {
        public FinalProductSpecifications(int id) : base(x=>x.Id == id)
        {
            AddInclude(x => x.Ingredients);
            AddInclude(x => x.Category);     
            AddInclude(x => x.SubCategory);  

        }

        public FinalProductSpecifications(IFinalProductSpecParams specParams, bool isCount) : base(BuildCriteria(specParams))
        {
            AddInclude(x => x.Ingredients);
            AddInclude(x => x.Category);     
            AddInclude(x => x.SubCategory);  

            if (!isCount)
            {
                ApplyPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
                if (string.IsNullOrEmpty(specParams.OrderBy) || specParams.OrderBy == "new")
                    AddOrderByDesc(x => x.Id);
                else AddOrderBy(x => x.Id);
            }

        }

        private static Expression<Func<FinalProduct, bool>> BuildCriteria(IFinalProductSpecParams specParams)
        {
            return x =>
                (string.IsNullOrEmpty(specParams.SearchName) || x.Name.ToLower().Contains(specParams.SearchName.ToLower())) &&
                (specParams.CategoryId == null || x.CategoryId == specParams.CategoryId) &&
                (specParams.SubCategoryId == null || x.SubCategoryId == specParams.SubCategoryId);

        }

        }
}
