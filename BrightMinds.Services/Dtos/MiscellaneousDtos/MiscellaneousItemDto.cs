using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrightMinds.Services.Dtos.MiscellaneousDtos
{
 
        public class MiscellaneousItemDto
        {
            [Required(ErrorMessage = "اسم المنتج مطلوب")]
            public string Name { get; set; }

            [Required(ErrorMessage = "العدد مطلوب")]
            [Range(1, int.MaxValue, ErrorMessage = "العدد يجب أن يكون أكبر من 0")]
            public int Quantity { get; set; }

            public decimal? Price { get; set; }  

            [Required(ErrorMessage = "اسم المستلم مطلوب")]
            public string ReceivedBy { get; set; }

            [Required(ErrorMessage = "مكان الاستلام مطلوب")]
            public string ReceivingPlace { get; set; }

            [Required(ErrorMessage = "تاريخ الاستلام مطلوب")]
            public DateTime ReceivingDate { get; set; }

            public IFormFile? Image { get; set; }   

            public string? Notes { get; set; }
        }
    
}
