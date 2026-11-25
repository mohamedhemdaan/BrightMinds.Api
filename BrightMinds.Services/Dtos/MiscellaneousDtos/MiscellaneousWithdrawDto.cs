using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrightMinds.Services.Dtos.MiscellaneousDtos
{
   
        public class MiscellaneousWithdrawDto
        {
            [Required]
            public int ItemId { get; set; }

            [Required(ErrorMessage = "اسم الساحب مطلوب")]
            public string WithdrawnBy { get; set; }

            [Required(ErrorMessage = "العدد المسحوب مطلوب")]
            [Range(1, int.MaxValue, ErrorMessage = "العدد يجب أن يكون أكبر من 0")]
            public int Quantity { get; set; }

            public string? Notes { get; set; }
        }
    
}
