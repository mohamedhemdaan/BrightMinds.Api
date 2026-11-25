using BrightMinds.Core.Enums;

namespace BrightMinds.Services.Dtos.RestaurantSupplyDtos
{
    public class RestaurantSupplyDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string ContainerUnitName { get; set; }
        public decimal ContainerSize { get; set; }
        public string? SubUnitName { get; set; }
        public decimal TotalContainerPrice { get; set; }
        public decimal PricePerUnit { get; set; }
        public int AvailableQuantity { get; set; }
        public decimal TotalWithdrawn { get; set; }

        public HandlingType HandlingType { get; set; }   
        public bool IsWeightBased { get; set; }   
        public decimal? TotalWeight { get; set; }       
    }
}