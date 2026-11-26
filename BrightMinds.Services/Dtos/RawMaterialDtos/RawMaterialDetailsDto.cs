using BrightMinds.Core.Enums;

namespace BrightMinds.Services.Dtos.RawMaterialDtos
{
    public class RawMaterialDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public HandlingType HandlingType { get; set; }

        // Purchase level (legacy response names retained).
        public string? ContainerUnitName { get; set; }
        public decimal ContainerSize { get; set; }
        public UnitType UnitType { get; set; }
        public string? SubUnitName { get; set; }
        public decimal? UnitWeightInKg { get; set; }
        public decimal TotalContainerPrice { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal? TotalWeightPerContainer { get; set; }
        public bool IsWeightBased { get; set; }

        public decimal NormalStockLevel { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool IsRestaurantSupply { get; set; }
        public bool IsMeat { get; set; }
        public decimal? MeatFactor { get; set; }
        public bool IsMultiplierLocked { get; set; }

        // Existing calculated/reference fields.
        public decimal? PricePerKg { get; set; }
        public string PriceDisplayUnit { get; set; }
        public decimal? PricePerPiece { get; set; }

        // Canonical three-level contract.
        public string? MiddleUnitName { get; set; }
        public string? SmallestUnitName { get; set; }
        public decimal SmallestUnitsPerMiddleUnit { get; set; }
        public decimal? PricePerMiddleUnit { get; set; }
        public decimal? PricePerSmallestUnit { get; set; }

        // Legacy inner-unit response fields retained for compatibility.
        public string? InnerUnitName { get; set; }
        public decimal InnerUnitsPerSubUnit { get; set; }
        public decimal? InnerUnitWeightInKg { get; set; }
        public decimal? PricePerInnerUnit { get; set; }
        public decimal? PricePerSubUnit { get; set; }
    }
}
