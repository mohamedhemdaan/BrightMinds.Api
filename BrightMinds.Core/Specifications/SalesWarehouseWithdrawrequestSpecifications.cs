using BrightMinds.Core.ISpecificationParams;
using BrightMinds.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace BrightMinds.Core.Specifications
{
    public class SalesWarehouseWithdrawrequestSpecifications : BaseSpecification<SalesWarehouseWithdrawRequest>
    {
        public SalesWarehouseWithdrawrequestSpecifications(int id) : base(x=>x.Id == id)
        {
            AddInclude(x => x.RequestedItems);
            AddIncludeString("RequestedItems.ProductPortion.Product");
            AddInclude(x => x.Shift);
        }
        public SalesWarehouseWithdrawrequestSpecifications(Shift shift) : base(x=>x.ShiftId == shift.Id)
        {
            AddInclude(x => x.RequestedItems);
            AddIncludeString("RequestedItems.ProductPortion.Product");
            AddInclude(x => x.Shift);
        }

        public SalesWarehouseWithdrawrequestSpecifications(ISalesWarehouseWithdrawRequestSpec specParams, bool isCount) : base(BuildCriteria(specParams))
        {
            AddInclude(x => x.RequestedItems);
            AddIncludeString("RequestedItems.ProductPortion.Product");
            AddInclude(x => x.Shift);
            if (!isCount)
            {
                ApplyPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
                if (string.IsNullOrEmpty(specParams.OrderBy) || specParams.OrderBy == "new")
                    AddOrderByDesc(x => x.Id);
                else AddOrderBy(x => x.Id);
            }

        }
        public SalesWarehouseWithdrawrequestSpecifications(ISalesWarehouseWithdrawRequestSpec specParams, bool isCount, string? senderId, string? recieverId) : base(BuildCriteria(specParams, senderId, recieverId)) 
        {
            AddInclude(x => x.RequestedItems);
            AddIncludeString("RequestedItems.ProductPortion.Product");
            AddInclude(x => x.Shift);
            if (!isCount)
            {
                ApplyPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
                if (string.IsNullOrEmpty(specParams.OrderBy) || specParams.OrderBy == "new")
                    AddOrderByDesc(x => x.Id);
                else AddOrderBy(x => x.Id);
            }
        }

        private static Expression<Func<SalesWarehouseWithdrawRequest, bool>> BuildCriteria(ISalesWarehouseWithdrawRequestSpec specParams)
        {
            return x => ((specParams.CreationDate == null || x.CreationDate == specParams.CreationDate) &&
                         (specParams.Status == null || x.Status == specParams.Status) &&
                         (specParams.CategoryId == null ||
                          x.RequestedItems.Any(item =>
                              item.InventoryItem.ProductPortion.CategoryId == specParams.CategoryId)));
        }
        private static Expression<Func<SalesWarehouseWithdrawRequest, bool>> BuildCriteria(ISalesWarehouseWithdrawRequestSpec specParams, string? senderId, string? recieverId)
        {
            return x => ((specParams.CreationDate == null || x.CreationDate == specParams.CreationDate)&&
                         (specParams.Status == null || x.Status == specParams.Status)&&
                         (senderId == null || x.SenderId == senderId)&&
                         (recieverId == null || x.ReceiverId == recieverId));
        }
    }
}
