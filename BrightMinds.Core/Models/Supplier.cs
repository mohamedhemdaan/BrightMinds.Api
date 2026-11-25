using System;

namespace BrightMinds.Core.Models
{
    public class Supplier : BaseEntity
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public virtual ICollection<Supply> Supplies { get; set; } = new HashSet<Supply>();
    }
}
