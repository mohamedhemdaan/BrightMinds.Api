using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;


namespace BrightMinds.Infrastructure.Repositories
{
    public class BaseUnitRepository : GenericRepository<BaseUnit>, IBaseUnitRepository
    {
        public BaseUnitRepository(AppDbContext context) : base(context)
        {
        }
    }
}
