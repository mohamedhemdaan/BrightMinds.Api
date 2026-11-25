using BrightMinds.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrightMinds.Core.Models
{

    public class RawMaterial : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }

        public HandlingType HandlingType { get; set; }

        public string? ContainerUnitName { get; set; }
        public decimal ContainerSize { get; set; }

        public UnitType UnitType { get; set; }

        public string? SubUnitName { get; set; }

        public decimal? UnitWeightInKg { get; set; }

        public decimal TotalContainerPrice { get; set; }

        public decimal NormalStockLevel { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public decimal PricePerUnit => ContainerSize > 0 ? TotalContainerPrice / ContainerSize : 0;
        public decimal? TotalWeightPerContainer => UnitWeightInKg.HasValue ? ContainerSize * UnitWeightInKg.Value : (decimal?)null;
        public bool IsWeightBased => UnitType == UnitType.kg || UnitType == UnitType.g;
        public bool IsCountBased => !IsWeightBased;
        public bool HasWeight => UnitWeightInKg.HasValue && UnitWeightInKg.Value > 0;

        public bool IsRestaurantSupply { get; set; } = false;

        public bool IsMeat { get; set; } = false;
        public decimal? MeatFactor { get; set; }
        public bool IsMultiplierLocked { get; set; } = false;

        // ✅ الحقول الجديدة (اختيارية، تُرسل فقط إذا كانت موجودة)
        public decimal? PricePerKg { get; set; }
        public decimal? PricePerPiece { get; set; }

        public virtual ICollection<RawMaterialUnit> Units { get; set; } = new List<RawMaterialUnit>();

        // ============================================================
        //  ✅ المستوى الثالث: الوحدة الداخلية الأصغر
        //  (داخل الوحدة الوسطى - SubUnit)
        //  المعادلة الموحّدة:
        //    PricePerInnerUnit = PricePerSubUnit / InnerUnitsPerSubUnit
        //  لو InnerUnitsPerSubUnit = 1 → السعر = سعر الوحدة الوسطى
        // ============================================================
        public string? InnerUnitName { get; set; }
        public decimal InnerUnitsPerSubUnit { get; set; } = 1;
        public decimal? InnerUnitWeightInKg { get; set; }
        public decimal? PricePerInnerUnit { get; set; }

    }

}
