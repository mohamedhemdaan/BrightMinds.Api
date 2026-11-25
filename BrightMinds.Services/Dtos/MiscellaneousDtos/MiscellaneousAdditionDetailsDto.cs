using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrightMinds.Services.Dtos.MiscellaneousDtos
{
    
        public class MiscellaneousAdditionDetailsDto
        {
            public int Id { get; set; }
            public int ItemId { get; set; }
            public string ItemName { get; set; }
            public string AddedBy { get; set; }
            public int Quantity { get; set; }
            public DateTime AdditionDate { get; set; }
            public string? Notes { get; set; }
        public int TotalAdded { get; set; }
    }
    }
