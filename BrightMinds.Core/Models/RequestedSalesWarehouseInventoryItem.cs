
using System.ComponentModel.DataAnnotations.Schema;

namespace BrightMinds.Core.Models
{
    public class RequestedSalesWarehouseInventoryItem : BaseEntity
    {
        [ForeignKey("ProductPortion")]
        public int ProductPortionId { get; set; }
        public ProductPortions ProductPortion { get; set; }

        [ForeignKey("InventoryItem")]
        public int? InventoryItemId { get; set; }
        public WarehouseProductsInventory InventoryItem { get; set; }
         
        [ForeignKey("WithdrawRequest")]
        public int WithdrawRequestId { get; set; }
        public SalesWarehouseWithdrawRequest WithdrawRequest { get; set; }

        public int Quantity { get; set; }
    }
}
