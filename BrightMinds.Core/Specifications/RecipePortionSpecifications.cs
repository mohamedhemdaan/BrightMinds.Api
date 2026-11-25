using BrightMinds.Core.ISpecificationParams;
using BrightMinds.Core.Models;
using System.Linq.Expressions;

namespace BrightMinds.Core.Specifications
{
    public class RecipePortionSpecifications : BaseSpecification<RecipePortion>
    {

        public RecipePortionSpecifications(int id):base(x=>x.Id == id)
        {
            AddInclude(x => x.Recipe);
            AddInclude(x => x.Category);     

        }

        public RecipePortionSpecifications(IRecipePortionsSpecParams specParams, bool isCount)
            : base(BuildCriteria(specParams))
        {
            AddInclude(x=>x.Recipe);
            AddInclude(x => x.Category);     

            if (!isCount)
            {
                ApplyPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
                if (string.IsNullOrEmpty(specParams.OrderBy) || specParams.OrderBy == "new")
                    AddOrderByDesc(x => x.Id);
                else AddOrderBy(x => x.Id);
            }

        }

        private static Expression<Func<RecipePortion, bool>> BuildCriteria(IRecipePortionsSpecParams specParams)
        {
            return x => ((specParams.RecipeName == null || x.Recipe.Name == specParams.RecipeName));
        }
    }
}
