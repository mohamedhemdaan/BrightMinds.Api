using BrightMinds.Core.Enums;

namespace BrightMinds.Core.ISpecificationParams
{
    public interface IShiftSpecParams
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string? OrderBy { get; set; }
        public string? EmployeeName { get; set; }
        public ShiftStatus? Status { get; set; }
        public DateTime? Date { get; set; }
    }
}
