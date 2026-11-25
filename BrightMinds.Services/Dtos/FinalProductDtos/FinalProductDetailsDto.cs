using BrightMinds.Services.Dtos.FinalProductIngredientDtos;

    namespace BrightMinds.Services.Dtos.FinalProductDtos
    {
        public class FinalProductDetailsDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal SellingPrice { get; set; }
            public decimal TaxPercentage { get; set; }
            public decimal PriceWithTax { get; set; }
            public decimal TotalCost { get; set; }
            public decimal TotalCostPrecentage { get; set; }

            public int? CategoryId { get; set; }
            public string? CategoryName { get; set; }
           
            public int? SubCategoryId { get; set; }
            public string? SubCategoryName { get; set; }

        public List<FinalProductIngredientDetailsDto> Ingredients { get; set; } = new List<FinalProductIngredientDetailsDto>();

            public List<ConsumedItem> ConsumedItems { get; set; } = new List<ConsumedItem>();
        }
    }
