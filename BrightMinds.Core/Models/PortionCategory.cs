using System.ComponentModel.DataAnnotations;

namespace BrightMinds.Core.Models
{
    public class PortionCategory : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}