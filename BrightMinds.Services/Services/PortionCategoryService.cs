using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;
using BrightMinds.Core.Specifications;
using BrightMinds.Services.Dtos.PortionCategoryDtos;
using BrightMinds.Services.IServices;
using Mapster;
using System.Net;

namespace BrightMinds.Services.Services
{
    public class PortionCategoryService : IPortionCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PortionCategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response> GetAllAsync()
        {
            var spec = new BaseSpecification<PortionCategory>(x => !x.IsDeleted);
            spec.AddOrderBy(x => x.Name);

            var data = await _unitOfWork.PortionCategoryRepository.GetAllWithSpecAsync(spec);
            var result = data.Adapt<List<PortionCategoryDto>>();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استرجاع تصنيفات البورشنات بنجاح",
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

            var existing = await _unitOfWork.PortionCategoryRepository
                .GetWithSpecAsync(new BaseSpecification<PortionCategory>(x => x.Name == name && !x.IsDeleted));

            if (existing != null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "هذا التصنيف موجود بالفعل"
                };

            var category = new PortionCategory { Name = name };
            await _unitOfWork.PortionCategoryRepository.Add(category);
            await _unitOfWork.CompleteAsync();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.Created,
                Message = "تم إضافة التصنيف بنجاح",
                Data = category.Adapt<PortionCategoryDto>()
            };
        }

        public async Task<Response> DeleteAsync(int id)
        {
            var category = await _unitOfWork.PortionCategoryRepository.GetAsync(id);
            if (category == null || category.IsDeleted)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "التصنيف غير موجود"
                };

            category.IsDeleted = true;
            _unitOfWork.PortionCategoryRepository.Update(category);
            await _unitOfWork.CompleteAsync();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم حذف التصنيف بنجاح"
            };
        }
    }
}