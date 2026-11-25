
namespace BrightMinds.Services.Dtos.WithdrawHistoryDtos
{
    public class WithdrawHistoryDetailsDto
    {
        public string? UserId { get; set; }
        public string RecipientName { get; set; }
        public DateTime Date { get; set; }
        public int NumberOfItems { get; set; }
        public List<WithdrawItemDetailsDto> WithdrawItems { get; set; }
    }
}
