
using BrightMinds.Core.Enums;

namespace BrightMinds.Services.Dtos.WarehouseDtos
{
    public class WarehouseDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public WarehouseType Type { get; set; }
    }
}
