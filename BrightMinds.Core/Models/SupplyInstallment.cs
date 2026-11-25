using System.ComponentModel.DataAnnotations.Schema;

namespace BrightMinds.Core.Models
{
    public class SupplyInstallment : BaseEntity
    {
        [ForeignKey("Supply")]
        public int SupplyId { get; set; }
        public Supply Supply { get; set; }

        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
