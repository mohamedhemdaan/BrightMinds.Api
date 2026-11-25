using BrightMinds.Services.Dtos.RawMaterialDtos;
using BrightMinds.Services.SpecificationParams;
using System;

namespace BrightMinds.Services.IServices
{
    public interface IRawMaterialService
    {
        public Task<Response> GetAllAsync(RawMaterialSpecParams specParams);
        public Task<Response> GetByIdAsync(int id);
        public Task<Response> CreateAsync(RawMaterialDto rawMaterialDto);
        public Task<Response> UpdateAsync(RawMaterialDto rawMaterialDto, int id);
        public Task<Response> DeleteAsync(int id);
    }
}
