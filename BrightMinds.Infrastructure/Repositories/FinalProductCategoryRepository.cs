using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;

namespace BrightMinds.Infrastructure.Repositories
{
    public class FinalProductCategoryRepository : GenericRepository<FinalProductCategory>, IFinalProductCategoryRepository
    {
        public FinalProductCategoryRepository(AppDbContext context) : base(context)
        {
        }
    }
}