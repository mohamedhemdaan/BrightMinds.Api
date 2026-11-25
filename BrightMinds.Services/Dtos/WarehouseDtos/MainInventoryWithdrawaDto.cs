
using BrightMinds.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace BrightMinds.Services.Dtos.WarehouseDtos
{
    public class MainInventoryWithdrawaDto
    {
        public string RecipientName { get; set; }
        [MinLength(1, ErrorMessage = "لازم طلب السحب يكون فيه على الاقل صنف واحد")]
        public List<WithdrawRequestDto> WithdrawRequests { get; set; }
    }

        public class WithdrawRequestDto
        {
            [Required]
            public int Id { get; set; }     

            [Required]
            [Range(0.01, double.MaxValue)]
            public decimal ContainerQuantity { get; set; }    
        }
    }
