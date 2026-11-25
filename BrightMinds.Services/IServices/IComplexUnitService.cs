using BrightMinds.Services.Dtos.UnitDtos;
using BrightMinds.Services.SpecificationParams;

namespace BrightMinds.Services.IServices
{
    public interface IComplexUnitService
    {
        public Task<Response> CreateComplexUnitAsync(ComplexUnitDto unitDto);
        public Task<Response> UpdateComplexUnitAsync(int id, ComplexUnitDto unitDto);
        public Task<Response> DeleteComplexUnitAsync(int id);
        public Task<Response> GetAsync(int id);
        public Task<Response> GetAll(UnitSpecParams specParams);
    }
}
