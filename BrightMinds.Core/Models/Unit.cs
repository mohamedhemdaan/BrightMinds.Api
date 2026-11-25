using BrightMinds.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrightMinds.Core.Models
{
    public class Unit : BaseEntity
    {
        public string Name { get; set; }
        public UomType Type { get; set; }

        public bool IsImmutable { get; set; } = false;
    }
}
