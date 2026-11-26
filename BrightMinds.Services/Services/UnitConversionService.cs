using BrightMinds.Core.Enums;
using BrightMinds.Core.Models;

namespace BrightMinds.Services.Services
{
    /// <summary>
    /// Canonical unit conversion and price lookup for a raw material.
    /// A middle unit is the existing stock unit described by UnitType; purchase
    /// quantities use ContainerSize and smallest quantities use the configured
    /// inner-unit count. This service is intentionally side-effect free.
    /// </summary>
    public static class UnitConversionService
    {
        public static bool TryConvertToMiddleUnit(
            RawMaterial material,
            decimal quantity,
            RawMaterialUnitLevel level,
            out decimal middleQuantity)
        {
            middleQuantity = 0;
            if (quantity < 0 || material == null)
                return false;

            switch (level)
            {
                case RawMaterialUnitLevel.Purchase:
                    if (material.ContainerSize <= 0) return false;
                    middleQuantity = quantity * material.ContainerSize;
                    return true;

                case RawMaterialUnitLevel.Middle:
                    middleQuantity = quantity;
                    return true;

                case RawMaterialUnitLevel.Smallest:
                    var smallestPerMiddle = material.SmallestUnitsPerMiddleUnit > 1
                        ? material.SmallestUnitsPerMiddleUnit
                        : material.InnerUnitsPerSubUnit;
                    if (smallestPerMiddle <= 0) return false;
                    middleQuantity = quantity / smallestPerMiddle;
                    return true;

                default:
                    return false;
            }
        }

        public static decimal? GetPricePerUnit(RawMaterial material, RawMaterialUnitLevel level)
        {
            if (material == null) return null;

            return level switch
            {
                RawMaterialUnitLevel.Purchase => material.TotalContainerPrice,
                RawMaterialUnitLevel.Middle => material.PricePerMiddleUnit
                    ?? (material.ContainerSize > 0 ? material.TotalContainerPrice / material.ContainerSize : null),
                RawMaterialUnitLevel.Smallest => material.PricePerSmallestUnit
                    ?? material.PricePerInnerUnit
                    ?? GetCalculatedSmallestPrice(material),
                _ => null
            };
        }

        public static string? GetUnitName(RawMaterial material, RawMaterialUnitLevel level)
        {
            if (material == null) return null;
            return level switch
            {
                RawMaterialUnitLevel.Purchase => material.ContainerUnitName,
                RawMaterialUnitLevel.Middle => material.MiddleUnitName ?? material.UnitType.ToString(),
                RawMaterialUnitLevel.Smallest => material.SmallestUnitName ?? material.InnerUnitName ?? material.SubUnitName,
                _ => null
            };
        }

        private static decimal? GetCalculatedSmallestPrice(RawMaterial material)
        {
            var middlePrice = material.PricePerMiddleUnit
                ?? (material.ContainerSize > 0 ? material.TotalContainerPrice / material.ContainerSize : null);
            var count = material.SmallestUnitsPerMiddleUnit > 1
                ? material.SmallestUnitsPerMiddleUnit
                : material.InnerUnitsPerSubUnit;
            return middlePrice.HasValue && count > 0 ? middlePrice.Value / count : null;
        }
    }
}
