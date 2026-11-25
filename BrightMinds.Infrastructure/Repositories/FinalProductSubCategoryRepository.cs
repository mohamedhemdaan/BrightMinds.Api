using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;

namespace BrightMinds.Infrastructure.Repositories
{
    public class FinalProductSubCategoryRepository : GenericRepository<FinalProductSubCategory>, IFinalProductSubCategoryRepository
    {
        public FinalProductSubCategoryRepository(AppDbContext context) : base(context)
        {
        }
    }
}