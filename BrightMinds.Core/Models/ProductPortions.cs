using BrightMinds.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrightMinds.Core.Models
{
    public class ProductPortions : BaseEntity
    {
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal SizeInGrams { get; set; }
        public ProductionStatus ProductionStatus { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? TransferredDate { get; set; }

        public int? CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public PortionCategory Category { get; set; }

    }
}
