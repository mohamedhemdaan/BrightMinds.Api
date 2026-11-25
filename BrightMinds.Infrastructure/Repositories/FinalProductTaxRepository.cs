using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BrightMinds.Infrastructure.Repositories
{
    public class FinalProductTaxRepository : GenericRepository<FinalProductTax>, IFinalProductTaxRepository
    {
        private readonly AppDbContext _context;
        public FinalProductTaxRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }


        public async Task<decimal> GetUnifiedTaxPrecentage()
        {
            var unifiedTax = await _context.FinalProductTaxs.FirstOrDefaultAsync();

            return unifiedTax?.TaxPrecentage ?? 14;
        }
    }
}
