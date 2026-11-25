
using System.ComponentModel.DataAnnotations.Schema;

namespace BrightMinds.Core.Models
{
    public class WarehouseProductsInventory : BaseEntity
    {
        [ForeignKey("ProductPortion")]
        public int ProductPortionId { get; set; }
        public ProductPortions ProductPortion { get; set; }

        [ForeignKey("Warehouse")]
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Size { get; set; }
    }
}
