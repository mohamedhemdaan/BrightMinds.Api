using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;

namespace BrightMinds.Infrastructure.Repositories
{
    public class PortionCategoryRepository : GenericRepository<PortionCategory>, IPortionCategoryRepository
    {
        public PortionCategoryRepository(AppDbContext context) : base(context)
        {
        }
    }
}