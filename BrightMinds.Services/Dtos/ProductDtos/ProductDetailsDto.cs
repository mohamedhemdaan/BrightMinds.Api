using BrightMinds.Core.Enums;

namespace BrightMinds.Services.Dtos.ProductDtos
{
    public class ProductDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RecipeId { get; set; }
        public decimal Cost { get; set; }
        public bool IsFlexible { get; set; }
        public DateTime CreationDate { get; set; }
        public ProductionStatus ProductionStatus { get; set; }
        public decimal ExpectedQuantity { get; set; }
        public decimal? ActualQuantity { get; set; }

        public decimal? InitialTotalWeight { get; set; }
        public decimal? SettledWeight { get; set; }
        public decimal? SettlementLoss { get; set; }

        public string? SettledByUserId { get; set; }
        public string? SettledByUserName { get; set; }   
        public DateTime? SettlementDate { get; set; }

        public List<ProductPortionsDto> ProductPortions { get; set; } = new List<ProductPortionsDto>();
    }

    public class ProductPortionsDto
    {
        public int Id { get; set; }
        public string Name {  set; get; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal SizeInGrams { get; set; }
        public ProductionStatus ProductionStatus { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? TransferredDate { get; set; }
    }
}
