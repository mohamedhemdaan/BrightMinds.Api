using BrightMinds.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrightMinds.Core.Models
{
    public class Shift : BaseEntity
    {
        [ForeignKey("User")]
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string EmployeeName { get; set; }

        public ShiftStatus Status { get; set; } = ShiftStatus.Opened;

        public DateTime StartTimeStamp { get; set; } = DateTime.UtcNow;
        public DateTime? EndTimeStamp { get; set; }
        public double? TotalHours { get; set; }
    }
}
