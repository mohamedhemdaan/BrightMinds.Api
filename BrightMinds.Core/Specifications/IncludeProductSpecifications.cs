using BrightMinds.Core.Models;

namespace BrightMinds.Core.Specifications
{
    public class IncludeProductSpecifications : BaseSpecification<ProductPortions>
    {
        public IncludeProductSpecifications(int? productPortionId):base(x=> (productPortionId == null || x.Id == productPortionId))
        {
            AddInclude(x => x.Product);
        }
    }
}
