using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrightMinds.Services.Dtos.MiscellaneousDtos
{
  
        public class MiscellaneousItemDetailsDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Quantity { get; set; }
            public decimal? Price { get; set; }
            public string ReceivedBy { get; set; }
            public string ReceivingPlace { get; set; }
            public DateTime ReceivingDate { get; set; }
            public string? ImageUrl { get; set; }
            public string? Notes { get; set; }
            public DateTime CreatedAt { get; set; }
            public int TotalWithdrawn { get; set; }      
            public int AvailableQuantity { get; set; }      

        public int TotalAdded { get; set; }             

    }

}
