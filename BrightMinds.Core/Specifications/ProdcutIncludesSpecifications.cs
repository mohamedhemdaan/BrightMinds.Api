
using BrightMinds.Core.Models;
using System.Linq.Expressions;

namespace BrightMinds.Core.Specifications
{
    public class ProdcutIncludesSpecifications : BaseSpecification<Product>
    {
        public ProdcutIncludesSpecifications(int? id = null) : base(BuildCriteria(id))
        {
            AddInclude(x => x.ProductPortions);
            AddInclude(x => x.Recipe);
            AddInclude(x => x.SettledByUser);
        } 

        private static Expression<Func<Product, bool>> BuildCriteria(int? id)
        {
            return x => (id == null || x.Id == id);
        }
    }
}
