using BrightMinds.Services.Dtos.FinalProductSubCategoryDtos;

namespace BrightMinds.Services.IServices
{
    public interface IFinalProductSubCategoryService
    {
        Task<Response> GetAllAsync();
        Task<Response> GetByCategoryIdAsync(int categoryId);
        Task<Response> CreateAsync(string name, int categoryId);
        Task<Response> DeleteAsync(int id);
    }
}