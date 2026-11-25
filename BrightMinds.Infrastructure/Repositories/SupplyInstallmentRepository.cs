using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;

namespace BrightMinds.Infrastructure.Repositories
{
    public class SupplyInstallmentRepository : GenericRepository<SupplyInstallment>, ISupplyInstallmentRepository
    {
        public SupplyInstallmentRepository(AppDbContext context) : base(context)
        {
        }
    }
}
