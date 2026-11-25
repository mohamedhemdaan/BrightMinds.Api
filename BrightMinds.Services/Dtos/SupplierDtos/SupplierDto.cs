using System.ComponentModel.DataAnnotations;

namespace BrightMinds.Services.Dtos.SupplierDtos
{
    public class SupplierDto
    {
        [Required]
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

    }
}
