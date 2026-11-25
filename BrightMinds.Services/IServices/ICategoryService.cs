
using BrightMinds.Services.Dtos.CategoryDtos;
using BrightMinds.Services.SpecificationParams;

namespace BrightMinds.Services.IServices
{
    public interface ICategoryService
    {
        public Task<Response> CreateAsync(CategoryDto categoryDto);
        public Task<Response> GetAllAsync(CategorySpecParams specParams);
        public Task<Response> GatByIdAsync(int id);
        public Task<Response> UpdateAsync(CategoryDto categoryDto, int id);
        public Task<Response> DeleteAsync(int id); 
    }
}
