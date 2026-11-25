using BrightMinds.Core.Models;

namespace BrightMinds.Core.Interfaces
{
    public interface IFinalProductTaxRepository : IGenericRepository<FinalProductTax>
    {
        public Task<decimal> GetUnifiedTaxPrecentage();
    }
}
