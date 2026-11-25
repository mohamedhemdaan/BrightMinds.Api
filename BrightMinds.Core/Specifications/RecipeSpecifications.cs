using BrightMinds.Core.ISpecificationParams;
using BrightMinds.Core.Models;
using System.Linq.Expressions;

namespace BrightMinds.Core.Specifications
{
    public class RecipeSpecifications : BaseSpecification<Recipe>
    {
        public RecipeSpecifications(IRecipeSpecParams? specParams, int? id, bool isCount) : base(BuildCriteria(specParams, id))
        {
            AddInclude(R => R.Ingrediants);
            AddInclude(R => R.Portions);

            AddInclude(R => R.PortionCategory);     

            AddIncludeString("Ingrediants.RawMaterial");
            AddIncludeString("Ingrediants.SubRecipe");

            if (!isCount && specParams != null)
            {
                ApplyPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
                if (string.IsNullOrEmpty(specParams.OrderBy) || specParams.OrderBy == "new")
                    AddOrderByDesc(x => x.Id);
                else AddOrderBy(x => x.Id);
            }
        }

        private static Expression<Func<Recipe, bool>> BuildCriteria(IRecipeSpecParams? specParams = null, int? id = null)
        {
            return x => (id == null || x.Id == id) &&
                        (x.IsDeleted == false) &&
                        (specParams == null ||
                         string.IsNullOrEmpty(specParams.SearchName) || x.Name.ToLower().Contains(specParams.SearchName.ToLower())) &&
                        (specParams == null ||
                         string.IsNullOrEmpty(specParams.Category) || x.Category == specParams.Category);
        }

    }
}
