using System.ComponentModel.DataAnnotations;

namespace BrightMinds.Services.Dtos.FinalProductSubCategoryDtos
{
    public class CreateFinalProductSubCategoryDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}