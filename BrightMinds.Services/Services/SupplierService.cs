using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;
using BrightMinds.Core.Specifications;
using BrightMinds.Services.Dtos.SupplierDtos;
using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Library.Services.Dtos;
using Mapster;
using System.Net;

namespace BrightMinds.Services.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public SupplierService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Response> GetAllAsync(SupplierSpecParams specParams)
        {
            var spec = new SupplierSpecifications(specParams, isCount: false);
            var data = await _unitOfWork.SupplierRepository.GetAllWithSpecAsync(spec);

            var countSpec = new SupplierSpecifications(specParams, isCount: true);
            var count = await _unitOfWork.SupplierRepository.GetCountWithSpecAsync(countSpec);

            var result = data.Adapt<List<SupplierDetailsDto>>();
            var pagination = new Pagination(specParams.PageSize, specParams.PageIndex, count, result);

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استرجاع بيانات الموَردين بنجاح",
                Data = pagination
            };
        }

        public async Task<Response> CreateAsync(SupplierDto supplierDto)
        {
            var supplier = supplierDto.Adapt<Supplier>();
            await _unitOfWork.SupplierRepository.Add(supplier);
            await _unitOfWork.CompleteAsync();
            var result = supplier.Adapt<SupplierDetailsDto>();
            return new Response()
            {
                StatusCode = (int)HttpStatusCode.Created,
                Message = "تم حفظ بيانات الموَرد بنجاح",
                Data = result
            };
        }

        public async Task<Response> DleteAsync(int id)
        {
            var supplier = await _unitOfWork.SupplierRepository.GetAsync(id);
            if (supplier == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "الموَرد مش موجود"
                };
            _unitOfWork.SupplierRepository.Delete(supplier);
            await _unitOfWork.CompleteAsync();
            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم حذف الموَرد بنجاح"
            };
        }

        public async Task<Response> GetByIdAsync(int id)
        {
            var supplier = await _unitOfWork.SupplierRepository.GetAsync(id);
            if (supplier == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "الموَرد مش موجود"
                };
            var result = supplier.Adapt<SupplierDetailsDto>();
            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استعادة بيانات المورد بنجاح",
                Data = result
            };
        }

        public async Task<Response> UpdateAsync(SupplierDto supplierDto, int id)
        {
            var supplier = await _unitOfWork.SupplierRepository.GetAsync(id);
            if (supplier == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "الموَرد مش موجود"
                };

            supplier.Name = supplierDto.Name;
            supplier.Address = supplierDto.Address;
            supplier.PhoneNumber = supplierDto.PhoneNumber;

            _unitOfWork.SupplierRepository.Update(supplier);
            await _unitOfWork.CompleteAsync();

            var result = supplier.Adapt<SupplierDetailsDto>();
            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم تحديث بيانات الموَرد بنجاح",
                Data = result
            };
        }
    }
}
