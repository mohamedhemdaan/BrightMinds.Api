using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrightMinds.Services.Dtos.SupplyInstallmentDtos
{
    public class SupplyInstallmentDetailsDto
    {
        public int Id { get; set; }
        public int SupplyId { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
