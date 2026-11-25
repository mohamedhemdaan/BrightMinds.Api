using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;
using BrightMinds.Core.Specifications;
using BrightMinds.Services.Dtos.FinalProductCategoryDtos;
using BrightMinds.Services.IServices;
using Mapster;
using System.Net;

namespace BrightMinds.Services.Services
{
    public class FinalProductCategoryService : IFinalProductCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FinalProductCategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response> GetAllAsync()
        {
            var spec = new BaseSpecification<FinalProductCategory>(x => !x.IsDeleted);
            spec.AddOrderBy(x => x.Name);

            var data = await _unitOfWork.FinalProductCategoryRepository.GetAllWithSpecAsync(spec);
            var result = data.Adapt<List<FinalProductCategoryDto>>();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استرجاع تصنيفات المنتجات النهائية بنجاح",
                Data = result
            };
        }

        public async Task<Response> CreateAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "اسم التصنيف مطلوب"
                };

            var existing = await _unitOfWork.FinalProductCategoryRepository
                .GetWithSpecAsync(new BaseSpecification<FinalProductCategory>(x => x.Name == name && !x.IsDeleted));

            if (existing != null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "هذا التصنيف موجود بالفعل"
                };

            var category = new FinalProductCategory { Name = name };
            await _unitOfWork.FinalProductCategoryRepository.Add(category);
            await _unitOfWork.CompleteAsync();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.Created,
                Message = "تم إضافة التصنيف بنجاح",
                Data = category.Adapt<FinalProductCategoryDto>()
            };
        }

        public async Task<Response> DeleteAsync(int id)
        {
            var category = await _unitOfWork.FinalProductCategoryRepository.GetAsync(id);
            if (category == null || category.IsDeleted)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "التصنيف غير موجود"
                };

            category.IsDeleted = true;
            _unitOfWork.FinalProductCategoryRepository.Update(category);
            await _unitOfWork.CompleteAsync();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم حذف التصنيف بنجاح"
            };
        }
    }
}