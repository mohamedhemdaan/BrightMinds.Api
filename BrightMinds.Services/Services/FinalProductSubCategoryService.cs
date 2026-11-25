using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;
using BrightMinds.Core.Specifications;
using BrightMinds.Services.Dtos.FinalProductSubCategoryDtos;
using BrightMinds.Services.IServices;
using Mapster;
using System.Net;

namespace BrightMinds.Services.Services
{
    public class FinalProductSubCategoryService : IFinalProductSubCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FinalProductSubCategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response> GetAllAsync()
        {
            var spec = new FinalProductSubCategorySpecifications(x => !x.IsDeleted);
            spec.AddOrderBy(x => x.Name);

            var data = await _unitOfWork.FinalProductSubCategoryRepository.GetAllWithSpecAsync(spec);
            var result = data.Adapt<List<FinalProductSubCategoryDto>>();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استرجاع التصنيفات الفرعية بنجاح",
                Data = result
            };
        }

        public async Task<Response> GetByCategoryIdAsync(int categoryId)
        {
            var category = await _unitOfWork.FinalProductCategoryRepository.GetAsync(categoryId);
            if (category == null || category.IsDeleted)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "التصنيف الرئيسي غير موجود"
                };

            var spec = new FinalProductSubCategorySpecifications(x => x.CategoryId == categoryId && !x.IsDeleted);
            spec.AddOrderBy(x => x.Name);

            var data = await _unitOfWork.FinalProductSubCategoryRepository.GetAllWithSpecAsync(spec);
            var result = data.Adapt<List<FinalProductSubCategoryDto>>();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استرجاع التصنيفات الفرعية لهذا التصنيف",
                Data = result
            };
        }

        public async Task<Response> CreateAsync(string name, int categoryId)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "اسم التصنيف الفرعي مطلوب"
                };

            var category = await _unitOfWork.FinalProductCategoryRepository.GetAsync(categoryId);
            if (category == null || category.IsDeleted)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "التصنيف الرئيسي غير موجود"
                };

            var existing = await _unitOfWork.FinalProductSubCategoryRepository
                .GetWithSpecAsync(new BaseSpecification<FinalProductSubCategory>(x => x.Name == name && x.CategoryId == categoryId && !x.IsDeleted));

            if (existing != null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "هذا التصنيف الفرعي موجود بالفعل تحت هذا التصنيف الرئيسي"
                };

            var subCategory = new FinalProductSubCategory
            {
                Name = name,
                CategoryId = categoryId
            };

            await _unitOfWork.FinalProductSubCategoryRepository.Add(subCategory);
            await _unitOfWork.CompleteAsync();

            var result = subCategory.Adapt<FinalProductSubCategoryDto>();
            result.CategoryName = category.Name;

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.Created,
                Message = "تم إضافة التصنيف الفرعي بنجاح",
                Data = result
            };
        }

        public async Task<Response> DeleteAsync(int id)
        {
            var subCategory = await _unitOfWork.FinalProductSubCategoryRepository.GetAsync(id);
            if (subCategory == null || subCategory.IsDeleted)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "التصنيف الفرعي غير موجود"
                };

            subCategory.IsDeleted = true;
            _unitOfWork.FinalProductSubCategoryRepository.Update(subCategory);
            await _unitOfWork.CompleteAsync();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم حذف التصنيف الفرعي بنجاح"
            };
        }
    }
}