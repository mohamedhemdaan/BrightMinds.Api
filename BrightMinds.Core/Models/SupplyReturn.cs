using BrightMinds.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrightMinds.Core.Models
{
    public class SupplyReturn : BaseEntity
    {
        [ForeignKey("User")]
        public string? UserId { get; set; }
        public AppUser User { get; set; }
        public string ReturnerName { get; set; }

        [ForeignKey("Supply")]
        public int SupplyId { get; set; }
        public Supply Supply { get; set; }

        public SupplyReturnStatus Status { get; set; } = SupplyReturnStatus.Pending;

        public bool IsApproved { get; set; } = false;
        public DateTime CreationDate { get; set; } // Has default value DateTime.UtcNow
        public DateTime? ApprovalDate { get; set; }

        public virtual ICollection<SupplyReturnItem> Items { get; set; } = new List<SupplyReturnItem>();
    }
}
