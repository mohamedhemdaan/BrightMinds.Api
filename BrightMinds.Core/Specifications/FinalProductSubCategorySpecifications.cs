using BrightMinds.Core.Models;
using System.Linq.Expressions;

namespace BrightMinds.Core.Specifications
{
    public class FinalProductSubCategorySpecifications : BaseSpecification<FinalProductSubCategory>
    {
        public FinalProductSubCategorySpecifications(Expression<Func<FinalProductSubCategory, bool>> criteria)
            : base(criteria)
        {
            AddInclude(x => x.Category);       
        }
    }
}