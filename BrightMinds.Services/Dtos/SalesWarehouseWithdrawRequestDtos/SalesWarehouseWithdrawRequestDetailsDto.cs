
using BrightMinds.Core.Enums;

namespace BrightMinds.Services.Dtos.SalesWarehouseWithdrawRequestDtos
{
    public class SalesWarehouseWithdrawRequestDetailsDto
    {
        public int Id { get; set; }
        public string RecipientName { get; set; }
        public DateTime Date { get; set; }
        public RequestStatus Status { get; set; }
        public List<RequestedItemDetailsDto> RequestedItems { get; set; }
    }

    public class RequestedItemDetailsDto
    {
        public int Id { get; set; }
        public int InventoryItemId { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; }
        public decimal Size { get; set; }
    }
}
