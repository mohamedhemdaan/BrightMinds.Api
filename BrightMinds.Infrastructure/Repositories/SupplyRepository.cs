using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;

namespace BrightMinds.Infrastructure.Repositories
{
    public class SupplyRepository : GenericRepository<Supply>, ISupplyRepository
    {
        private readonly AppDbContext _context;
        public SupplyRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
