using BrightMinds.Core.Enums;
using BrightMinds.Services.Dtos.SalesWarehouseWithdrawRequestDtos;
using BrightMinds.Services.Dtos.ShiftDtos;
using BrightMinds.Services.Dtos.WithdrawHistoryDtos;

namespace BrightMinds.Services.Dtos.ReportDtos
{

    public class ShiftProductConsumptionDto
    {
        public string ProductName { get; set; }
        public decimal RequestedQuantity { get; set; }
        public decimal ConsumedQuantity { get; set; }
        public decimal RemainingQuantity { get; set; }
        public UnitType UnitType { get; set; }

        public decimal? InitialWeight { get; set; }
        public decimal? SettlementLoss { get; set; }
        public decimal? FinalWeight { get; set; }
        public string? SettledByUser { get; set; }
        public DateTime? SettlementDate { get; set; }
    }
    public class ChefShiftRepostDetailsDto
    {
        public ShiftDetailsDto ShiftDetails { get; set; }
        public int WithdrawRequestsCount { get; set; }
        public int AcceptedWithdrawRequestsCount { get; set; }
        public int RejectedWithdrawRequestsCount { get; set; }
        public int PendingWithdrawRequestsCount { get; set; }

        public List<ShiftProductConsumptionDto> ConsumptionSummary { get; set; }

        public List<SalesWarehouseWithdrawRequestDetailsDto> WithdrawRequests { get; set; }
        public List<WithdrawHistoryDetailsDto> WithdrawHistory { get; set; }
    }
}
