
namespace BrightMinds.Services.Dtos.RecipePortionDtos
{
    public class RecipePortionsDetailsDto
    {
        public int RecipeId { get; set; }
        public string RecipeName { get; set; }
        public List<PortionDetailsDto> Portions { get; set; } = new List<PortionDetailsDto>();
    }

    public class PortionDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal SizeInGrams { get; set; }
    }
}
