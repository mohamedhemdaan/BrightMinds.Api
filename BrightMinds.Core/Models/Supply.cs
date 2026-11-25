using BrightMinds.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrightMinds.Core.Models
{
    public class Supply : BaseEntity
    {
        public string RecipientName { get; set; }
        public DateTime CreationDate { get; set; }

        [ForeignKey("Supplier")]
        public int? SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
        public string SupplierName { get; set; }
        public decimal TotalCost { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal RefundAmount { get; set; }
        public SupplyPaymentStatus Status { get; set; } = SupplyPaymentStatus.NotPaid;
        public virtual ICollection<SupplyItem> SupplyItems { get; set; } = new HashSet<SupplyItem>();
        public virtual ICollection<SupplyInstallment> Installments { get; set; } = new HashSet<SupplyInstallment>();
        public virtual ICollection<SupplyReturn> SupplyReturns { get; set; } = new List<SupplyReturn>();
    }

    public static class SupplyExtentions
    {

        public static Supply EmbedEqualSupplyItems(this Supply supply)
        {
            if (supply == null || !supply.SupplyItems.Any())
                return supply;

            var merged = supply.SupplyItems
                               .GroupBy(s => new { s.RawMaterialId })
                               .Select(g =>
                               {
                                   var firstItem = g.First();
                                   var rawMaterial = firstItem.RawMaterial;      

                                   var totalContainerQuantity = g.Sum(si => si.ContainerQuantity);
                                   var totalSubUnitQuantity = g.Sum(si => si.Quantity);

                                   var unitType = rawMaterial?.UnitType ?? firstItem.UnitType;
                                   var containerUnitName = rawMaterial?.ContainerUnitName ?? "وحدة";
                                   var itemName = rawMaterial?.Name ?? firstItem.Name ?? "غير محدد";
                                   var isWeightBased = unitType == UnitType.kg || unitType == UnitType.g;

                                   return new SupplyItem()
                                   {
                                       UnitPrice = firstItem.UnitPrice,
                                       UnitType = unitType,                              
                                       Quantity = totalSubUnitQuantity,
                                       ContainerQuantity = totalContainerQuantity,
                                       SupplyId = supply.Id,
                                       RawMaterialId = g.Key.RawMaterialId,
                                       Name = itemName,                                  
                                       DisplayDetails = isWeightBased ?
                                           $"{totalContainerQuantity:F2} {containerUnitName} ({totalSubUnitQuantity:F3} {unitType})" :
                                           $"{totalContainerQuantity:F0} {containerUnitName} = {totalSubUnitQuantity:F0} {unitType}"
                                   };
                               }).ToList();

            supply.SupplyItems = merged;
            return supply;
        }

    }

}
