using BrightMinds.Services.Dtos.SupplyDtos;
using BrightMinds.Services.Dtos.SupplyInstallmentDtos;
using BrightMinds.Services.SpecificationParams;

namespace BrightMinds.Services.IServices
{
    public interface ISupplyService
    {
        public Task<Response> GetAllAsync(SupplySpecParams specParams);
        public Task<Response> GetByIdAsync(int id);
        public Task<Response> CreateAsync(SupplyDto supplyDto);
        public Task<Response> UpdateAsync(SupplyDto supplyDto, int id);
        public Task<Response> DeleteAsync(int id);
        public Task<Response> CreateSupplyInstallmentAsync(int supplyId, SupplyInstallmentDto  supplyInstallmentDto);
        public Task<Response> UpdateSupplyInstallmentAsync(int supplyInstalmentId, SupplyInstallmentDto  supplyInstallmentDto);
        public Task<Response> DeleteSupplyInstallmentAsync(int supplyInstalmentId);
        public Task<Response> GetSupplyInstallmentByIdAsync(int supplyInstalmentId);
        public Task<Response> GetSupplyInstallmentsAsync(SupplyInstallmentSpecParams speParams);
    }
}
