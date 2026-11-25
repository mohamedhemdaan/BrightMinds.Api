using BrightMinds.Core.Models;

namespace BrightMinds.Core.Interfaces
{
    public interface IShiftRepository : IGenericRepository<Shift>
    {
        public Task<Shift?> GetLatestShiftForUser(string userId);
    }
}
