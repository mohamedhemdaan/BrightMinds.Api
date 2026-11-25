using BrightMinds.Core.Enums;

namespace BrightMinds.Services.Dtos.WarehouseDtos
{

        public class WarehouseInventoryItemDetailsDto
        {
            public int Id { get; set; }
            public string InventoryItemName { get; set; }
            public decimal Quantity { get; set; }   
            public decimal ContainerQuantity { get; set; }   
            public string ContainerUnitName { get; set; }
            public string DisplayDetails { get; set; }       
            public decimal Price { get; set; }
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public string UnitType { get; set; }
            public StockStatus StockStatus { get; set; }

            public string? SubUnitName { get; set; }
            public decimal? TotalWeightInKg { get; set; }
            public HandlingType HandlingType { get; set; }
            
            public decimal ContainerPrice { get; set; }          
            public decimal ContainerSize { get; set; }            

    }
}
