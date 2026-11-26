using BrightMinds.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace BrightMinds.Services.Dtos.RawMaterialDtos
{
    public class RawMaterialDto
    {
        [Required(ErrorMessage = "الاسم مطلوب")]
        public string Name { get; set; }
        public string? Description { get; set; }

        [Required(ErrorMessage = "نوع التعامل مطلوب")]
        public HandlingType HandlingType { get; set; }

        // Legacy fields are kept so existing frontend clients remain compatible.
        [Required(ErrorMessage = "وحدة الشراء مطلوبة")]
        public string ContainerUnitName { get; set; }
        [Range(1, double.MaxValue, ErrorMessage = "عدد الوحدات يجب أن يكون أكبر من 0")]
        public decimal ContainerSize { get; set; }
        [Required(ErrorMessage = "نوع الوحدة مطلوب")]
        public UnitType UnitType { get; set; }
        public string? SubUnitName { get; set; }
        public decimal? UnitWeightInKg { get; set; }

        [Required(ErrorMessage = "السعر الكلي للحاوية مطلوب")]
        [Range(0.01, double.MaxValue, ErrorMessage = "السعر يجب أن يكون أكبر من 0")]
        public decimal TotalContainerPrice { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public decimal NormalStockLevel { get; set; }
        public bool IsRestaurantSupply { get; set; } = false;
        public bool IsMeat { get; set; } = false;
        public decimal? MeatFactor { get; set; }
        public bool IsMultiplierLocked { get; set; } = false;

        // New canonical middle/smallest fields. ContainerSize remains the count
        // of middle units in the purchase unit for this API version.
        public string? MiddleUnitName { get; set; }
        public string? SmallestUnitName { get; set; }
        public decimal SmallestUnitsPerMiddleUnit { get; set; } = 1;
        public decimal? PricePerMiddleUnit { get; set; }
        public decimal? PricePerSmallestUnit { get; set; }

        // Legacy aliases retained for clients already using the inner-unit model.
        public string? InnerUnitName { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "عدد الوحدات الصغرى يجب أن يكون أكبر من 0")]
        public decimal InnerUnitsPerSubUnit { get; set; } = 1;
        public decimal? InnerUnitWeightInKg { get; set; }
    }
}
