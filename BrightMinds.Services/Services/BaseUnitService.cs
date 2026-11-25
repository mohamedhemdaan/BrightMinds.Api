using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;
using BrightMinds.Core.Specifications;
using BrightMinds.Services.Dtos.UnitDtos;
using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Library.Services.Dtos;
using Mapster;
using System.Net;

namespace BrightMinds.Services.Services
{
    public class BaseUnitService : IBaseUnitService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BaseUnitService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response> CreateBaseUnitAsync(BaseUnitDto unitDto)
        {
            var category = await _unitOfWork.UomCategoryRepository.GetAsync(unitDto.CategoryId);
            if (category == null)
                return new Response()
                {
                    Message = "تصنيف وحدة القياس مش موجود",
                    StatusCode = (int)HttpStatusCode.NotFound
                };

            var unit = unitDto.Adapt<BaseUnit>();
            await _unitOfWork.BaseUnitRepository.Add(unit);
            await _unitOfWork.CompleteAsync();

            return new Response()
            {
                Message = "تم انشاء وحدة القياس بنجاح",
                StatusCode = (int)HttpStatusCode.Created
            };
        }

        public async Task<Response> GetAsync(int id)
        {
            var unit = await _unitOfWork.BaseUnitRepository.GetWithSpecAsync(new BaseUnitSpecifications(id));
            if (unit == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "وحدة القياس مش موجودة"
                };

            var result = unit.Adapt<BaseUnitDetailsDto>();
            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استعادة وحدة القياس بنجاح",
                Data = result
            };

        }

        public async Task<Response> GetAll(UnitSpecParams specParams)
        {
            var spec = new BaseUnitSpecifications(specParams, false);
            var data = await _unitOfWork.BaseUnitRepository.GetAllWithSpecAsync(spec);

            var countSpec = new BaseUnitSpecifications(specParams, true);
            var count = await _unitOfWork.BaseUnitRepository.GetCountWithSpecAsync(countSpec);

            var result = data.Adapt<List<BaseUnitDetailsDto>>();
            var pagination = new Pagination(specParams.PageSize, specParams.PageIndex, count, result);

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استعادة بيانات وحدات القياس بنجاح",
                Data = pagination
            };
        }

        public async Task<Response> UpdateBaseUnitAsync(int id, BaseUnitDto unitDto)
        {
            var unit = await _unitOfWork.BaseUnitRepository.GetAsync(id);
            if (unit == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "وحدة القياس مش موجودة"
                };

            if (unit.IsImmutable)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "الوحدة غير قابلة للتعديل"
                };

            var category = await _unitOfWork.UomCategoryRepository.GetAsync(unitDto.CategoryId);
            if (category == null)
                return new Response()
                {
                    Message = "تصنيف وحدة القياس مش موجود",
                    StatusCode = (int)HttpStatusCode.NotFound
                };

            unit.Name = unitDto.Name;
            unit.CategoryId = unit.CategoryId;
            unit.ConversionFactor = unitDto.ConversionFactor;
            _unitOfWork.BaseUnitRepository.Update(unit);
            await _unitOfWork.CompleteAsync();

            var result = unit.Adapt<BaseUnitDetailsDto>();
            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم تحديث وحدة القياس بنجاح",
                Data = result
            };
        }

        public async Task<Response> DeleteBaseUnitAsync(int id)
        {
            var unit = await _unitOfWork.BaseUnitRepository.GetAsync(id);
            if (unit == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "وحدة القياس مش موجودة"
                };

            if (unit.IsImmutable)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "الوحدة غير قابلة للتعديل"
                };

            _unitOfWork.BaseUnitRepository.Delete(unit);
            await _unitOfWork.CompleteAsync();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم حذف وحدة القياس بنجاح"
            };
        }

    }
}
