using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;


namespace BrightMinds.Infrastructure.Repositories
{
    public class ComplexUnitRepository : GenericRepository<ComplexUnit>, IComplexUnitRepository
    {
        public ComplexUnitRepository(AppDbContext context) : base(context)
        {
        }
    }
}
