using BrightMinds.Services.Dtos.UnitDtos;
using BrightMinds.Services.SpecificationParams;

namespace BrightMinds.Services.IServices
{
    public interface IBaseUnitService
    {
        public Task<Response> CreateBaseUnitAsync(BaseUnitDto unitDto);
        public Task<Response> UpdateBaseUnitAsync(int id, BaseUnitDto unitDto);
        public Task<Response> DeleteBaseUnitAsync(int id);
        public Task<Response> GetAsync(int id);
        public Task<Response> GetAll(UnitSpecParams specParams);

    }
}
