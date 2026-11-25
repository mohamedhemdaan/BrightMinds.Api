using BrightMinds.Core.Models;
using BrightMinds.Core.Specifications;

public class SupplyWithItemsSpecification : BaseSpecification<Supply>
{
    public SupplyWithItemsSpecification(int id) : base(s => s.Id == id)
    {
        AddInclude(s => s.SupplyItems);
        AddInclude(s => s.Installments);
        AddInclude(s => s.Supplier);
        AddIncludeString("SupplyItems.RawMaterial");
    }
}