using BrightMinds.Core.Enums;

namespace BrightMinds.Core.Models
{
    public class Warehouse : BaseEntity
    {
        public string Name { get; set; }
        public WarehouseType Type { get; set; }

        public virtual ICollection<WarehouseSuppliesInventory> Inventory { get; set; }
    }
}
