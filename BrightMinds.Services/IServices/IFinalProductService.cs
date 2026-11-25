using BrightMinds.Services.Dtos.FinalProductDtos;
using BrightMinds.Services.SpecificationParams;

namespace BrightMinds.Services.IServices
{
    public interface IFinalProductService
    {
        public Task<Response> CreateFinalProduct(FinalProductDto finalProductDto);
        public Task<Response> UpdateFinalProduct(FinalProductDto finalProductDto, int id);
        public Task<Response> DeleteFinalProduct(int id);
        public Task<Response> GetAllFinalProducts(FinalProductSpecParams specParams);
        public Task<Response> GetFinalProductById(int id);

    }
}
