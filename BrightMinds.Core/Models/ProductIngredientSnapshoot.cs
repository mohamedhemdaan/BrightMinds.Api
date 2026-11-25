using BrightMinds.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrightMinds.Core.Models
{
    public class ProductIngredientSnapshoot : BaseEntity
    {
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public UnitType UnitType { get; set; }
        public decimal Price { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
