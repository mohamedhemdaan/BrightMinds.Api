using BrightMinds.Core.ISpecificationParams;
using BrightMinds.Core.Models;
using System.Linq.Expressions;

namespace BrightMinds.Core.Specifications
{
    public class SupplyInstallmentSpecifications : BaseSpecification<SupplyInstallment>
    {
        public SupplyInstallmentSpecifications(ISupplyInstallmentSpecParams specParams, bool isCount)
            : base(BuildCriteria(specParams))
        {
            if (!isCount)
            {
                ApplyPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
                if (string.IsNullOrEmpty(specParams.OrderBy) || specParams.OrderBy == "new")
                    AddOrderByDesc(x => x.Id);
                else AddOrderBy(x => x.Id);
            }
        }

        private static Expression<Func<SupplyInstallment, bool>> BuildCriteria(ISupplyInstallmentSpecParams specParams)
        {
            return x => ((specParams.SupplyId == null || x.SupplyId == specParams.SupplyId)&&
                         (specParams.Date == null || x.PaymentDate == specParams.Date));
        }

    }
}
