using BrightMinds.Core.Models;

namespace BrightMinds.Core.Interfaces
{
    public interface ISupplyReturnRepository : IGenericRepository<SupplyReturn>
    {
        public Task<List<SupplyReturn>> GetAllSupplyReturnsWithSupplyId(int supplyId);
    }
}
