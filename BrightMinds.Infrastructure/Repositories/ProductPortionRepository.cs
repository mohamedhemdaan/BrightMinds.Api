using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;

namespace BrightMinds.Infrastructure.Repositories
{
    public class ProductPortionRepository : GenericRepository<ProductPortions>, IProductPortionRepository
    {
        public ProductPortionRepository(AppDbContext context) : base(context)
        {
        }
    }
}
