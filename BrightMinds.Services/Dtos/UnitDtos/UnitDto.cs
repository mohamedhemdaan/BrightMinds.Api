using System.ComponentModel.DataAnnotations;

namespace BrightMinds.Services.Dtos.UnitDtos
{
    public class UnitDto
    {
        [Required]
        public string Name { get; set; }

    }
}
