
using BrightMinds.Core.Models;

namespace BrightMinds.Core.Specifications
{
    public class RecipePortionIncludesSpecifications : BaseSpecification<RecipePortion>
    {
        public RecipePortionIncludesSpecifications(int id) : base(x=>x.Id == id)
        {
            AddInclude(x => x.Recipe);
        }
    }
}
