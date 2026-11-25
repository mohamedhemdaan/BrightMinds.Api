using System;
using System.ComponentModel.DataAnnotations.Schema;
using BrightMinds.Core.Enums;
using BrightMinds.Core.Models;

namespace BrightMinds.Core.Models
{

    public class SupplyItem : BaseEntity
    {
        public string Name { get; set; }

        public decimal Quantity { get; set; }

        public decimal? ContainerQuantity { get; set; }

        public string? DisplayDetails { get; set; }

        public decimal UnitPrice { get; set; }    

        [ForeignKey("Supply")]
        public int SupplyId { get; set; }
        public virtual Supply Supply { get; set; }

        [ForeignKey("RawMaterial")]
        public int RawMaterialId { get; set; }
        public virtual RawMaterial RawMaterial { get; set; }

        public UnitType UnitType { get; set; }

        public decimal? ReceivedWeightKg { get; set; }          
        public decimal? ExchangePrice { get; set; }          
        public decimal? PriceMultiplier { get; set; }       

        public virtual ICollection<WarehouseSuppliesInventory> warehouses { get; set; }

        //  ✅ جديد: طريقة الاستلام (حاوية / وحدة صغرى)
        // ============================================================
        public ReceivingMethod ReceivedBy { get; set; } = ReceivingMethod.Container;

        // ============================================================
        //  ✅ جديد: القيم الأصلية للمستخدم (للتدقيق)
        // ============================================================
        public decimal? OriginalQuantity { get; set; }   // الكمية الأصلية اللي المستخدم دخلها
        public decimal? OriginalPrice { get; set; }      // السعر الأصلي اللي المستخدم دخله

    }
}