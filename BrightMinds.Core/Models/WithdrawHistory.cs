
using System.ComponentModel.DataAnnotations.Schema;

namespace BrightMinds.Core.Models
{
    public class WithdrawHistory : BaseEntity
    {
        [ForeignKey("User")]
        public string? UserId { get; set; }
        public virtual AppUser User { get; set; }

        [ForeignKey("Warehouse")]
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }
        public string RecipientName { get; set; }

        [ForeignKey("ChefShift")]
        public int? ChefShiftId { get; set; }
        public Shift ChefShift { get; set; }
        
        public DateTime Date { get; set; }
        public int NumberOfItems { get; set; }
        public virtual List<WithdrawItem> WithdrawItems { get; set; } = new List<WithdrawItem>();
    }
}
