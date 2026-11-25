
using System.ComponentModel.DataAnnotations;

namespace BrightMinds.Services.Dtos.ProductDtos
{
    public class ProductQuantitiesDto : IValidatableObject
    {
        [Required]
        public int ProductId { get; set; }
        public int? ActualQuantity { get; set; }
        public decimal? ActualSizeInGrams { get; set; }

        public List<ProductQuantityDto>? ProductQuantities { get; set; } = new List<ProductQuantityDto>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(ActualSizeInGrams == null && ProductQuantities?.Count>0)
            {
                yield return new ValidationResult("لازم تدخل الحجم الفعلى للمنتج", new[] { "ActualSize" });
            }

            if(ActualSizeInGrams != null && ProductQuantities?.Count==0)
            {
                yield return new ValidationResult("لازم تدخل احجام البرسيلات والكيميات بتاعتها", new[] { "ActualSize" });
            }

            if(ActualSizeInGrams != null && ActualQuantity != null)
            {
                yield return new ValidationResult("مينفعش تدخل الحجم الفعلى للمنتج وعدد البرسيلات مع بعض", new[] { "ActualSize" });
            }
        }
    }

    public class ProductQuantityDto
    {
        [Required]
        public int NumberOfPortions { get; set; }
        [Required]
        public int PortionId { get; set; }
    }
}
