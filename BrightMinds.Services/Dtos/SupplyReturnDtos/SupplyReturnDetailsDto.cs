
using BrightMinds.Core.Enums;

namespace BrightMinds.Services.Dtos.SupplyReturnDtos
{
    public class SupplyReturnDetailsDto
    {
        public int Id { get; set; }
        public string ReturnerName { get; set; }
        public int SupplyId { get; set; }
        public SupplyReturnStatus Status { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public List<SupplyReturnItemDetailsDto> Items { get; set; } = new List<SupplyReturnItemDetailsDto>();
    }

    public class SupplyReturnItemDetailsDto
    {
        public int Id { get; set; }
        public int SupplyReturnId { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public UnitType UnitType { get; set; }
        public decimal Price { get; set; }
    }
}
