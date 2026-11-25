using System.ComponentModel.DataAnnotations;

namespace BrightMinds.Services.Dtos.SupplyReturnDtos
{
    public class SupplyReturnDto
    {
        [Required]
        public int SupplyId { get; set; }

        [MinLength(1, ErrorMessage = "لازم يكزن فيه عنصر مرتجع واحد")]
        public List<SupplyReturnItemDto> Items { get; set; }
    }

    public class SupplyReturnItemDto
    {
        [Required]
        public int SupplyItemId { get; set; }
        
        [Required]
        public decimal Quantity { get; set; }
    }
}
