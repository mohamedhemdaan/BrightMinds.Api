using BrightMinds.Core.Enums;

namespace BrightMinds.Core.ISpecificationParams
{
    public interface IProductionSpecParams
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string? SearchName { get; set; }
        public string? OrderBy { get; set; }
        public DateTime? ProductionDate { get; set; }
        public ProductionStatus? ProductionStatus { get; set; }
    }
}
