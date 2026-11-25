using BrightMinds.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrightMinds.Core.Models
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }        
        
        [ForeignKey("Recipe")]
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
        public decimal Cost { get; set; }
        public DateTime CreationDate { get; set; }
        public ProductionStatus ProductionStatus { get; set; }
        public decimal? ExpectedQuantity { get; set; }
        public decimal? ActualQuantity { get; set; }
        public bool IsFlexible { get; set; }

        public decimal? InitialTotalWeight { get; set; }         
        public decimal? SettledWeight { get; set; }              
        public decimal? SettlementLoss { get; set; }            

        public string? SettledByUserId { get; set; }
        [ForeignKey(nameof(SettledByUserId))]
        public virtual AppUser SettledByUser { get; set; }
        public DateTime? SettlementDate { get; set; }

        public int? WithdrawHistoryId { get; set; }
        [ForeignKey(nameof(WithdrawHistoryId))]
        public virtual WithdrawHistory WithdrawHistory { get; set; }

        public virtual ICollection<ProductIngredientSnapshoot> Ingredients { get; set; } = new List<ProductIngredientSnapshoot>();
        public virtual ICollection<ProductPortions> ProductPortions { get; set; } = new List<ProductPortions>();
    }
}
