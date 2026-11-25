
using BrightMinds.Core.Enums;

namespace BrightMinds.Services.Dtos.ReportDtos
{
    public class RawMaterialReportDetailsDto
    {
        public decimal TotalQuantity { get; set; }
        public decimal QuantityInMainWarehouse { get; set; }
        public decimal QuantityInSubWarehouse { get; set; }
        public UnitType UnitType { get; set; }
        public List<RawMaterialInSupplyDetailsDto> InfoPerSupply { get; set; } = new List<RawMaterialInSupplyDetailsDto>();
        public List<RawMaterialBySupplierDetailsDto> InfoPerSupplier { get; set; } = new List<RawMaterialBySupplierDetailsDto>();
    }

    public class RawMaterialInSupplyDetailsDto
    {
        public string SupplierName { get; set; }
        public DateTime SupplyDate { get; set; }
        public decimal Quantity { get; set; }
        public UnitType UnitType { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class RawMaterialBySupplierDetailsDto
    {
        public string SupplierName { get; set; }
        public decimal TotalQuantitySupplied { get; set; }
    }
}
