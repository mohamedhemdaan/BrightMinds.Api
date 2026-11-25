using BrightMinds.Core.Interfaces;
using BrightMinds.Services.IServices;
using System.Net;

namespace BrightMinds.Services.Services
{
    public class UnitService : IUnitService
    {
        private readonly IUnitOfWork _uintOfWork;
        public IBaseUnitService BaseUnitService { get; }
        public IComplexUnitService ComplexUnitService { get; }

        public UnitService(IUnitOfWork unitOfWork, IBaseUnitService baseUnitService, IComplexUnitService complexUnitService)
        {
            _uintOfWork = unitOfWork;
            this.BaseUnitService = baseUnitService;
            this.ComplexUnitService = complexUnitService;
        }

        public async Task<Response> GetAllUnitCategories()
        {
            var categories = await _uintOfWork.UomCategoryRepository.GetAllAsync();
            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استعادة كل تصنيفات الوحدات بنجاح",
                Data = categories
            };
        }
    }
}
