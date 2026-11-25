using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;

namespace BrightMinds.Infrastructure.Repositories
{
    public class FinalProductRepository : GenericRepository<FinalProduct>, IFinalProductRepository
    {
        public FinalProductRepository(AppDbContext context) : base(context)
        {
        }
    }
}
