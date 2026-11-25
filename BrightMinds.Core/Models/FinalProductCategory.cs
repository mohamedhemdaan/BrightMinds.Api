using System.ComponentModel.DataAnnotations;

namespace BrightMinds.Core.Models
{
    public class FinalProductCategory : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}