using BrightMinds.Core.Enums;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.ISpecificationParams;
using BrightMinds.Core.Models;
using System.Linq.Expressions;

namespace BrightMinds.Core.Specifications
{
    public class WithdrawHistorySpecifications : BaseSpecification<WithdrawHistory>
    {
        public WithdrawHistorySpecifications(Shift shift) : base(x => x.ChefShiftId == shift.Id)
        {
            AddInclude(x => x.WithdrawItems);
        }

        public WithdrawHistorySpecifications(IWithdrawHistorySpecParams specParams, bool isCount)
            : base(BuildCriteria(specParams))
        {
            AddInclude(x => x.WithdrawItems);
            if (!isCount)
            {
                ApplyPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
                if (string.IsNullOrEmpty(specParams.OrderBy) || specParams.OrderBy == "new")
                    AddOrderByDesc(x => x.Id);
                else AddOrderBy(x => x.Id);
            }
        }

        private static Expression<Func<WithdrawHistory, bool>> BuildCriteria(IWithdrawHistorySpecParams specParams)
        {
            return x =>
                (string.IsNullOrEmpty(specParams.RecipientSearchName) || x.RecipientName.ToLower().Contains(specParams.RecipientSearchName.ToLower())) &&
                (specParams.WarehouseId == null || x.WarehouseId == specParams.WarehouseId) &&
                (specParams.FromDate == null || x.Date.Date >= specParams.FromDate.Value.Date) &&
                (specParams.ToDate == null || x.Date.Date <= specParams.ToDate.Value.Date)&&
                        (specParams.ShiftId == null || x.ChefShiftId == specParams.ShiftId);
;
        }
    }
}