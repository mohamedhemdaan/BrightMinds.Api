using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;

namespace BrightMinds.Infrastructure.Repositories
{
    public class UomCategoryRepository : GenericRepository<UomCategory>, IUomCategoryRepository
    {
        public UomCategoryRepository(AppDbContext context) : base(context)
        {
        }
    }
}
