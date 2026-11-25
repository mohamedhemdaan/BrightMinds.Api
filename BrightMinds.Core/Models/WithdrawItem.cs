using BrightMinds.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrightMinds.Core.Models
{

        public class WithdrawItem : BaseEntity
        {
            [ForeignKey("WithdrawHistory")]
            public int WithdrawHistoryId { get; set; }
            public virtual WithdrawHistory WithdrawHistory { get; set; }

            public string Name { get; set; }
            public decimal Quantity { get; set; }    
            public decimal? ContainerQuantity { get; set; }    
            public UnitType UnitType { get; set; }
            public string DisplayDetails { get; set; }

            public decimal? ConsumedQuantity { get; set; }          
            public int? SourceWithdrawItemId { get; set; }           
    }
    
}
