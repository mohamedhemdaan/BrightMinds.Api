using BrightMinds.Services.Dtos.PortionCategoryDtos;

namespace BrightMinds.Services.IServices
{
    public interface IPortionCategoryService
    {
        Task<Response> GetAllAsync();
        Task<Response> CreateAsync(string name);
        Task<Response> DeleteAsync(int id);
    }
}