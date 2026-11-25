
using BrightMinds.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrightMinds.Core.Models
{
    public class SalesWarehouseWithdrawRequest : BaseEntity
    {
        [ForeignKey("SenderUser")]
        public string? SenderId { get; set; }
        public AppUser SenderUser { get; set; }
        public string RecipientName { get; set; }

        [ForeignKey("ReceiverUser")]
        public string? ReceiverId { get; set; }
        public AppUser ReceiverUser { get; set; }

        [ForeignKey("Shift")]
        public int ShiftId { get; set; }
        public Shift Shift { get; set; }

        public RequestStatus Status { get; set; }
        public DateTime CreationDate { get; set; }

        public virtual List<RequestedSalesWarehouseInventoryItem> RequestedItems { get; set; } = new List<RequestedSalesWarehouseInventoryItem>();
    }
}
