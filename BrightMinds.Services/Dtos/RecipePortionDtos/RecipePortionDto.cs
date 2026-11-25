using System.ComponentModel.DataAnnotations;

namespace BrightMinds.Services.Dtos.RecipePortionDtos
{
    public class RecipePortionDto
    {
        [Required]
        public int RecipeId { get; set; }

        [MinLength(1, ErrorMessage = "لازم يكون فيه عنصر من البورشنات واحد على الاقل")]
        public List<PortionDto> Portions { get; set; }
    }

    public class PortionDto
    {
        public string Name { get; set; }
        public decimal SizeInGrams { get; set; }

        public int? CategoryId { get; set; }      

    }
}
