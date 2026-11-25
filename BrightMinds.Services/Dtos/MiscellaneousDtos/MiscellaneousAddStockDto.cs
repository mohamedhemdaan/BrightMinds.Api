using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrightMinds.Services.Dtos.MiscellaneousDtos
{
    using System.ComponentModel.DataAnnotations;

        public class MiscellaneousAddStockDto
        {
            [Required]
            public int ItemId { get; set; }

            [Required(ErrorMessage = "اسم المُضيف مطلوب")]
            public string AddedBy { get; set; }

            [Required(ErrorMessage = "الكمية المضافة مطلوبة")]
            [Range(1, int.MaxValue)]
            public int Quantity { get; set; }

            public string? Notes { get; set; }
        }
    }