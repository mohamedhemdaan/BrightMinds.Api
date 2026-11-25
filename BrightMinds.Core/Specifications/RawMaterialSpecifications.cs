#region old

#endregion

using BrightMinds.Core.ISpecificationParams;
using BrightMinds.Core.Models;
using System.Linq.Expressions;

namespace BrightMinds.Core.Specifications
{
    public class RawMaterialSpecifications : BaseSpecification<RawMaterial>
    {
        public RawMaterialSpecifications(IRawMaterialSpecParams specParams, bool isCount)
            : base(BuildCriteria(specParams))
        {

            AddInclude(x => x.Category);

            if (!isCount)
            {
                ApplyPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);

                if (!string.IsNullOrEmpty(specParams.OrderBy))
                {
                    switch (specParams.OrderBy.ToLower())
                    {
                        case "name":
                            AddOrderBy(x => x.Name);
                            break;
                        case "nameDesc":
                            AddOrderByDesc(x => x.Name);
                            break;
                        case "pricePerUnit":
                            AddOrderBy(x => x.TotalContainerPrice / (x.ContainerSize == 0 ? 1 : x.ContainerSize));
                            break;
                        case "pricePerUnitDesc":
                            AddOrderByDesc(x => x.TotalContainerPrice / (x.ContainerSize == 0 ? 1 : x.ContainerSize));
                            break;
                        default:
                            AddOrderByDesc(x => x.Id);
                            break;
                    }
                }
                else
                {
                    AddOrderByDesc(x => x.Id);
                }
            }
        }

        private static Expression<Func<RawMaterial, bool>> BuildCriteria(IRawMaterialSpecParams specParams)
        {
            return x => ((string.IsNullOrEmpty(specParams.SearchName) || x.Name.ToLower().Contains(specParams.SearchName)) &&
                         (specParams.CategoryId == null || x.CategoryId == specParams.CategoryId));
        }
    }
}