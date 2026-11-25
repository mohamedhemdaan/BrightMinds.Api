
using BrightMinds.Services.Dtos.SupplierDtos;
using BrightMinds.Services.SpecificationParams;

namespace BrightMinds.Services.IServices
{
    public interface ISupplierService
    {
        public Task<Response> GetAllAsync(SupplierSpecParams specParams);
        public Task<Response> GetByIdAsync(int id);
        public Task<Response> CreateAsync(SupplierDto supplierDto);
        public Task<Response> UpdateAsync(SupplierDto supplierDto, int id);
        public Task<Response> DleteAsync(int id);
    }
}
