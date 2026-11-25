using BrightMinds.Core.ISpecificationParams;
using BrightMinds.Core.Models;
using System.Linq.Expressions;


namespace BrightMinds.Core.Specifications
{
    public class ShiftSpecifications : BaseSpecification<Shift>
    {
        public ShiftSpecifications(IShiftSpecParams specParams, bool isCount) : base(BuildCriteria(specParams))
        {
            if (!isCount)
            {
                ApplyPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
                if (string.IsNullOrEmpty(specParams.OrderBy) || specParams.OrderBy == "new")
                    AddOrderByDesc(x => x.Id);
                else AddOrderBy(x => x.Id);
            }

        }

        private static Expression<Func<Shift, bool>> BuildCriteria(IShiftSpecParams specParams)
        {
            return x => ((specParams.EmployeeName == null || x.EmployeeName == specParams.EmployeeName) &&
                       (specParams.Status == null || x.Status == specParams.Status) &&
                       (specParams.Date == null || x.StartTimeStamp.Date == specParams.Date.Value.Date));
        }
    }
}
