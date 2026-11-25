using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrightMinds.Core.Models
{
    
        public class MiscellaneousAddition : BaseEntity
        {
            [ForeignKey("Item")]
            public int ItemId { get; set; }
            public virtual MiscellaneousItem Item { get; set; }

            public string AddedBy { get; set; }                  
            public int Quantity { get; set; }                 
            public DateTime AdditionDate { get; set; }        
            public string? Notes { get; set; }                 
        }
    }