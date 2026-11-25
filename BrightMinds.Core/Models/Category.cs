using System;

namespace BrightMinds.Core.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public virtual ICollection<RawMaterial> Products { get; set; } = new HashSet<RawMaterial>();
    }
}
