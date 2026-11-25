
using System.ComponentModel.DataAnnotations;

namespace BrightMinds.Services.Dtos.WarehouseDtos
{
    public class SalesInventoryWithdrawDto
    {
        [Required(ErrorMessage = "لازم تدخل المعرف بتاع طلب السحب من مخزن البيع")]
        public int WithdrawRequestId { get; set; }

        [MinLength(1, ErrorMessage = "لازم طلب السحب يكون فيه على الاقل صنف واحد")]
        public List<ProductWithdrawDto> Products { get; set; }
    }

    public class ProductWithdrawDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
