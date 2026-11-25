using BrightMinds.Core.Enums;

namespace BrightMinds.Services.Dtos.ShiftDtos
{
    public class ShiftDetailsDto
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public ShiftStatus Status { get; set; }

        public DateTime StartTimeStamp { get; set; }
        public DateTime? EndTimeStamp { get; set; }
        public double? TotalHours { get; set; }

    }
}
