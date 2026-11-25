
namespace BrightMinds.Services.IServices
{
    public interface IUnitService
    {
        public IBaseUnitService BaseUnitService { get; }
        public IComplexUnitService ComplexUnitService { get; }

        public Task<Response> GetAllUnitCategories();
    }
}
