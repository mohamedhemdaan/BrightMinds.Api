using BrightMinds.Core.Enums;

namespace BrightMinds.Services.Dtos.RestaurantSupplyDtos
{
    public class RestaurantSupplyWithdrawDetailsDto
    {
        public int Id { get; set; }
        public int RawMaterialId { get; set; }
        public string RawMaterialName { get; set; }
        public string WithdrawnBy { get; set; }
        public int Quantity { get; set; }
        public DateTime WithdrawDate { get; set; }
        public string? Notes { get; set; }
    }
}