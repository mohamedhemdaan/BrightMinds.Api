
using BrightMinds.Core.Models;

namespace BrightMinds.Core.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        public Task<List<Product>> GetDailyProduction();
    }
}
