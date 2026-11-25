using System.ComponentModel.DataAnnotations;

namespace BrightMinds.Services.Dtos.SalesWarehouseWithdrawRequestDtos
{
    public class SalesWarehouseWithdrawRequestDto 
    {

        public string RecipientName { get; set; }
        [MinLength(1, ErrorMessage = "لازم الطلب يحتوي على عنصر واحد على الاقل")]
        public List<RequestedItemDto> RequestedItems { get; set; }
    }

    public class RequestedItemDto
    {
        [Required]
        public int InventoryItemId { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
