using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrightMinds.Core.Models
{
    public class FinalProductSubCategory : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public FinalProductCategory Category { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}