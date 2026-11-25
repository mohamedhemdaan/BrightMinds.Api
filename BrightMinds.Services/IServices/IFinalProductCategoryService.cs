using BrightMinds.Services.Dtos.FinalProductCategoryDtos;

namespace BrightMinds.Services.IServices
{
    public interface IFinalProductCategoryService
    {
        Task<Response> GetAllAsync();
        Task<Response> CreateAsync(string name);
        Task<Response> DeleteAsync(int id);
    }
}