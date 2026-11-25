
using System.ComponentModel.DataAnnotations;

namespace BrightMinds.Services.Dtos.UnitDtos
{
    public class BaseUnitDto : UnitDto
    {

        [Required]
        public int CategoryId { get; set; }
        [Required]
        public float? ConversionFactor { get; set; }
    }
}
