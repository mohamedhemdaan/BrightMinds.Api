using BrightMinds.Core.Enums;

namespace BrightMinds.Services.Dtos.WithdrawHistoryDtos
{
    public class WithdrawItemDetailsDto
    {
        public int Id { get; set; }

        public int WithdrawHistoryId { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public UnitType UnitType { get; set; }

        public decimal? ConsumedQuantity { get; set; }    

        public int? SourceWithdrawItemId { get; set; }   

    }
}
