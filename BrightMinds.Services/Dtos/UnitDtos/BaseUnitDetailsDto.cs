
namespace BrightMinds.Services.Dtos.UnitDtos
{
    public class BaseUnitDetailsDto : UnitDetailsDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public float? ConversionFactor { get; set; }     

    }
}
