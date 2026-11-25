using BrightMinds.Core.ISpecificationParams;
using BrightMinds.Core.Models;
using System.Linq.Expressions;

namespace BrightMinds.Core.Specifications
{
    public class SupplySpecifications : BaseSpecification<Supply>
    {
        public SupplySpecifications(ISupplySpecParams specParams, bool isCount)
            : base(BuildCriteria(specParams))
        {
            AddInclude(s => s.SupplyItems);
            AddInclude(s => s.Installments);
            AddInclude(s => s.Supplier);
            AddIncludeString("SupplyItems.RawMaterial");

            if (!isCount)
            {
                ApplyPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
                if (string.IsNullOrEmpty(specParams.OrderBy) || specParams.OrderBy == "new")
                    AddOrderByDesc(x => x.Id);
                else AddOrderBy(x => x.Id);
            }
        }

        private static Expression<Func<Supply, bool>> BuildCriteria(ISupplySpecParams specParams)
        {
            return x =>
                (string.IsNullOrEmpty(specParams.RecipientName) || x.RecipientName.ToLower().Contains(specParams.RecipientName.ToLower())) &&
                (specParams.PaymentStatus == null || x.Status == specParams.PaymentStatus) &&
                // ✅ إضافة فلتر التاريخ
                (specParams.FromDate == null || x.CreationDate.Date >= specParams.FromDate.Value.Date) &&
                (specParams.ToDate == null || x.CreationDate.Date <= specParams.ToDate.Value.Date);
        }
    }
}