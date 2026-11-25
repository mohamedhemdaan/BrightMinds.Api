using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrightMinds.Services.Dtos.MiscellaneousDtos
{
    
        public class MiscellaneousWithdrawDetailsDto
        {
            public int Id { get; set; }
            public int ItemId { get; set; }
            public string ItemName { get; set; }
            public string WithdrawnBy { get; set; }
            public int Quantity { get; set; }
            public DateTime WithdrawDate { get; set; }
            public string? Notes { get; set; }
        }
    
}
