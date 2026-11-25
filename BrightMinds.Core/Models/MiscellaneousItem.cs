using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrightMinds.Core.Models
{
   
        public class MiscellaneousItem : BaseEntity
        {
            public string Name { get; set; }                      
            public int Quantity { get; set; }                       
            public decimal? Price { get; set; }                        
            public string ReceivedBy { get; set; }                   
            public string ReceivingPlace { get; set; }               
            public DateTime ReceivingDate { get; set; }           

            public string? ImageUrl { get; set; }                  
            public string? Notes { get; set; }                   

            public string? CreatedBy { get; set; }                   
            public DateTime CreatedAt { get; set; }               

            public bool IsDeleted { get; set; } = false;          

            public virtual ICollection<MiscellaneousWithdraw> Withdraws { get; set; } = new List<MiscellaneousWithdraw>();

        public virtual ICollection<MiscellaneousAddition> Additions { get; set; } = new List<MiscellaneousAddition>();
    }
   
}
