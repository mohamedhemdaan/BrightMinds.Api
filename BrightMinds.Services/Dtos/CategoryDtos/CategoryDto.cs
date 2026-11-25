
using System.ComponentModel.DataAnnotations;

namespace BrightMinds.Services.Dtos.CategoryDtos
{
    public class CategoryDto
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
