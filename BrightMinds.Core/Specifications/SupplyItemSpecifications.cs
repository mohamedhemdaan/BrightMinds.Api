using BrightMinds.Core.Models;
using BrightMinds.Core.Specifications;

public class SupplyItemSpecifications : BaseSpecification<SupplyItem>
{
    public SupplyItemSpecifications(int rawMaterialId) : base(x => x.RawMaterialId == rawMaterialId)
    {
    }
}