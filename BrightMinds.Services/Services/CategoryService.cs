using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;
using BrightMinds.Core.Specifications;
using BrightMinds.Services.Dtos.CategoryDtos;
using BrightMinds.Services.Dtos.RawMaterialDtos;
using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Library.Services.Dtos;
using Mapster;
using System.Net;

namespace BrightMinds.Services.Services
{
    public class CategoryService : ICategoryService 
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response> CreateAsync(CategoryDto categoryDto)
        {
            Category category = categoryDto.Adapt<Category>();
            await _unitOfWork.CategoryRepository.Add(category);
            await _unitOfWork.CompleteAsync();

            var categoryResponse = category.Adapt<CategoryDetailsDto>();
            return new Response()
            {
                StatusCode = (int)HttpStatusCode.Created,
                Message = "تم انشاء التصنيف بنجاح",
                Data = categoryResponse
            };
        }

        public async Task<Response> DeleteAsync(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetAsync(id);
            if (category == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "التصنيف مش موجود!",
                    Data = null
                };

            _unitOfWork.CategoryRepository.Delete(category);
            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch(Exception ex)
            {
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "لا يمكن حذف الفئة"
                };
            }

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم حذف التصنيف بنجاح"
            };
        }

        public async Task<Response> GatByIdAsync(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetAsync(id);
            if (category == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "التصنيف مش موجود"
                };

            var result = category.Adapt<CategoryDetailsDto>();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استرجاع بيانات التصنيف بنجاح",
                Data = result
            };
        }

        public async Task<Response> GetAllAsync(CategorySpecParams specParams)
        {
            var spec = new CategorySpecifications(specParams, isCount: false); 
            var data = await _unitOfWork.CategoryRepository.GetAllWithSpecAsync(spec);

            var countSpec = new CategorySpecifications(specParams, isCount: true);
            var count = await _unitOfWork.CategoryRepository.GetCountWithSpecAsync(countSpec);

            var result = data.Adapt<List<CategoryDetailsDto>>();
            var pagination = new Pagination(specParams.PageSize, specParams.PageIndex, count, result);

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استعادة كافة التصنيفات بنجاح",
                Data = pagination
            };
        }

        public async Task<Response> UpdateAsync(CategoryDto categoryDto, int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetAsync(id);
            if (category == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "التصنيف مش موجود"
                };

            category.Name = categoryDto.Name;
            category.Description = categoryDto.Description;

            _unitOfWork.CategoryRepository.Update(category);
            await _unitOfWork.CompleteAsync();

            var result = category.Adapt<CategoryDetailsDto>();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم تحديث التصنيف بنجاح",
                Data = result
            };
        }
    }
}
