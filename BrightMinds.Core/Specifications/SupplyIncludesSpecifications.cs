using BrightMinds.Core.Models;

namespace BrightMinds.Core.Specifications
{
    public class SupplyIncludesSpecifications : BaseSpecification<Supply>
    {
        public SupplyIncludesSpecifications(int id) : base(x=>x.Id == id)
        {
            AddInclude(x => x.Installments);
            AddInclude(x => x.SupplyItems);
        }

    }
}
