using BrightMinds.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrightMinds.Core.Models
{

        public class WarehouseSuppliesInventory : BaseEntity
        {
            [ForeignKey("Warehouse")]
            public int WarehouseId { get; set; }
            public virtual Warehouse Warehouse { get; set; }

            [ForeignKey("SupplyItem")]
            public int SupplyItemId { get; set; }
            public SupplyItem SupplyItem { get; set; }

            public decimal Quantity { get; set; }

            public decimal? ContainerQuantity { get; set; }

            public string? DisplayDetails { get; set; }

            public decimal Price { get; set; }    
            public UnitType UnitType { get; set; }   
        }
    }
