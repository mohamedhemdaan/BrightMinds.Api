using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BrightMinds.Infrastructure.Repositories
{
    public class ShiftRepository : GenericRepository<Shift>, IShiftRepository
    {
        private readonly AppDbContext _context;
        public ShiftRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Shift?> GetLatestShiftForUser(string userId)
        {
            var lastShift = await _context.Shifts.Where(x => x.UserId == userId)
                                                 .OrderByDescending(x => x.StartTimeStamp).
                                                 ThenByDescending(x => x.Id)
                                                 .FirstOrDefaultAsync();
            return lastShift;
        }
    }
}
