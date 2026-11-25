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
    public class ComplexUnitService : IComplexUnitService
    {
        private readonly IUnitOfWork _uintOfWork; 

        public ComplexUnitService(IUnitOfWork unitOfWork)
        {
            _uintOfWork = unitOfWork;
        }

        public async Task<Response> CreateComplexUnitAsync(ComplexUnitDto unitDto)
        {
            var unit = unitDto.Adapt<ComplexUnit>();
            await _uintOfWork.ComplexUnitRepository.Add(unit);
            await _uintOfWork.CompleteAsync();

            var result = unit.Adapt<ComplexUnitDetailsDto>();
            return new Response()
            {
                StatusCode = (int)HttpStatusCode.Created,
                Message = "تم اضافة الوحدة المُركبة بنجاح",
                Data = result
            };
        }
  
        public async Task<Response> GetAll(UnitSpecParams specParams)
        {
            var spec = new ComplexUnitSpecifications(specParams, false);
            var data = await _uintOfWork.ComplexUnitRepository.GetAllWithSpecAsync(spec);

            var countSpec = new ComplexUnitSpecifications(specParams, true);
            var count = await _uintOfWork.ComplexUnitRepository.GetCountWithSpecAsync(countSpec);

            var result = data.Adapt<List<ComplexUnitDetailsDto>>();
            var pagination = new Pagination(specParams.PageSize, specParams.PageIndex, count, result);

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استعادة بيانات وحدات القياس المُركبة بنجاح",
                Data = pagination
            };
        }

        public async Task<Response> GetAsync(int id)
        {
            var unit = await _uintOfWork.ComplexUnitRepository.GetAsync(id);
            if (unit == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "وحدة القياس مش موجودة"
                };

            var result = unit.Adapt<ComplexUnitDetailsDto>();
            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استعادة وحدة القياس بنجاح",
                Data = result
            };
        }

        public async Task<Response> DeleteComplexUnitAsync(int id)
        {
            var unit = await _uintOfWork.ComplexUnitRepository.GetAsync(id);
            if (unit == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "وحدة القياس مش موجودة"
                };

            _uintOfWork.ComplexUnitRepository.Delete(unit);
            await _uintOfWork.CompleteAsync();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم حذف وحدة القياس بنجاح"
            };
        }

        public async Task<Response> UpdateComplexUnitAsync(int id, ComplexUnitDto unitDto)
        {
            var unit = await _uintOfWork.ComplexUnitRepository.GetAsync(id);
            if (unit == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "وحدة القياس مش موجودة"
                };

            unit.Name = unitDto.Name;
            _uintOfWork.ComplexUnitRepository.Update(unit);
            await _uintOfWork.CompleteAsync();

            var result = unit.Adapt<ComplexUnitDetailsDto>();
            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم تعديل وحدة القياس بنجاح",
                Data = result
            };
        }
    }
}
