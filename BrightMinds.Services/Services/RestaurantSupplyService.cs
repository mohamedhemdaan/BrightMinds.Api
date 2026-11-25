using BrightMinds.Core.Enums;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.ISpecificationParams;
using BrightMinds.Core.Models;
using BrightMinds.Core.Specifications;
using BrightMinds.Services.Dtos.RestaurantSupplyDtos;
using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Library.Services.Dtos;
using Mapster;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Security.Claims;

namespace BrightMinds.Services.Services
{
    public class RestaurantSupplyService : IRestaurantSupplyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public RestaurantSupplyService(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Response> GetAllSuppliesAsync(PaginationSpecParams specParams)
        {
            var spec = new BaseSpecification<RawMaterial>(x =>
                x.IsRestaurantSupply == true
                );

            spec.ApplyPagination(
                specParams.PageSize * (specParams.PageIndex - 1),
                specParams.PageSize
            );
            spec.AddOrderBy(x => x.Name);

            var data = await _unitOfWork.RawMaterialRepository.GetAllWithSpecAsync(spec);
            var count = await _unitOfWork.RawMaterialRepository.GetCountWithSpecAsync(spec);

            var result = new List<RestaurantSupplyDetailsDto>();

            foreach (var item in data)
            {
                var inventoryItem = await _unitOfWork.WarehouseSuppliesInventoryRepository
                    .GetFromSubWarehouseAsync(item.Id);

                var dto = item.Adapt<RestaurantSupplyDetailsDto>();
                dto.AvailableQuantity = inventoryItem != null ? (int)inventoryItem.Quantity : 0;
                dto.TotalWithdrawn = await GetTotalWithdrawnAsync(item.Id);
                dto.HandlingType = item.HandlingType;
                dto.IsWeightBased = item.IsWeightBased;
                dto.TotalWeight = item.UnitWeightInKg.HasValue && inventoryItem != null
                    ? inventoryItem.Quantity * item.UnitWeightInKg.Value
                    : (decimal?)null;
                result.Add(dto);
            }
            var pagination = new Pagination(specParams.PageSize, specParams.PageIndex, count, result);

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استرجاع منتجات الصالة بنجاح",
                Data = pagination
            };
        }

        public async Task<Response> GetSupplyByIdAsync(int id)
        {
            var rawMaterial = await _unitOfWork.RawMaterialRepository.GetAsync(id);
            if (rawMaterial == null || !rawMaterial.IsRestaurantSupply)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المنتج غير موجود أو ليس منتج صالة"
                };

            var inventoryItem = await _unitOfWork.WarehouseSuppliesInventoryRepository
                .GetFromSubWarehouseAsync(id);

            var result = rawMaterial.Adapt<RestaurantSupplyDetailsDto>();
            result.AvailableQuantity = inventoryItem != null ? (int)inventoryItem.Quantity : 0;
            result.TotalWithdrawn = await GetTotalWithdrawnAsync(id);

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استرجاع المنتج بنجاح",
                Data = result
            };
        }

        public async Task<Response> WithdrawAsync(RestaurantSupplyWithdrawDto dto)
        {
            var rawMaterial = await _unitOfWork.RawMaterialRepository.GetAsync(dto.RawMaterialId);
            if (rawMaterial == null || !rawMaterial.IsRestaurantSupply)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المنتج غير موجود أو ليس منتج صالة"
                };

            var inventoryItem = await _unitOfWork.WarehouseSuppliesInventoryRepository
                .GetFromSubWarehouseAsync(dto.RawMaterialId);

            if (inventoryItem == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المنتج غير موجود في المخزن الفرعي"
                };

            decimal quantityToDeduct = dto.Quantity;

            if (rawMaterial.HandlingType == HandlingType.CountAndWeight && dto.IsWithdrawByWeight)
            {
                if (!rawMaterial.UnitWeightInKg.HasValue || rawMaterial.UnitWeightInKg.Value <= 0)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "وزن الوحدة غير محدد لهذا المنتج"
                    };

                quantityToDeduct = dto.Quantity / rawMaterial.UnitWeightInKg.Value;
            }

            if (inventoryItem.Quantity < quantityToDeduct)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = $"الكمية المتاحة غير كافية (المتاح: {inventoryItem.Quantity})"
                };

            inventoryItem.Quantity -= quantityToDeduct;

            if (rawMaterial.ContainerSize > 0)
            {
                inventoryItem.ContainerQuantity = inventoryItem.Quantity / rawMaterial.ContainerSize;
            }

            if (rawMaterial.UnitWeightInKg.HasValue)
            {
                var totalWeight = inventoryItem.Quantity * rawMaterial.UnitWeightInKg.Value;
                inventoryItem.DisplayDetails = rawMaterial.IsWeightBased ?
                    $"{inventoryItem.ContainerQuantity:F2} {rawMaterial.ContainerUnitName} ({inventoryItem.Quantity:F3} {rawMaterial.UnitType})" :
                    $"{inventoryItem.ContainerQuantity:F0} {rawMaterial.ContainerUnitName} = {inventoryItem.Quantity:F0} {rawMaterial.SubUnitName ?? rawMaterial.UnitType.ToString()} ({totalWeight:F3} كجم)";
            }
            else
            {
                inventoryItem.DisplayDetails = rawMaterial.IsWeightBased ?
                    $"{inventoryItem.ContainerQuantity:F2} {rawMaterial.ContainerUnitName} ({inventoryItem.Quantity:F3} {rawMaterial.UnitType})" :
                    $"{inventoryItem.ContainerQuantity:F0} {rawMaterial.ContainerUnitName} = {inventoryItem.Quantity:F0} {rawMaterial.SubUnitName ?? rawMaterial.UnitType.ToString()}";
            }

            if (inventoryItem.Quantity == 0)
                _unitOfWork.WarehouseSuppliesInventoryRepository.Delete(inventoryItem);
            else
                _unitOfWork.WarehouseSuppliesInventoryRepository.Update(inventoryItem);

            var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = _contextAccessor.HttpContext?.User?.Identity?.Name ?? "غير معروف";

            var firstName = _contextAccessor.HttpContext?.User?.FindFirstValue("FirstName");
            var lastName = _contextAccessor.HttpContext?.User?.FindFirstValue("LastName");
            if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
                userName = $"{firstName} {lastName}";

            var withdraw = new RestaurantSupplyWithdraw()
            {
                RawMaterialId = dto.RawMaterialId,
                WithdrawnBy = dto.WithdrawnBy,
                Quantity = quantityToDeduct,
                WithdrawDate = DateTime.UtcNow,
                Notes = dto.Notes,
                IsWithdrawnByWeight = dto.IsWithdrawByWeight,
                WithdrawnWeight = dto.IsWithdrawByWeight ? dto.Quantity : (decimal?)null
            };

            await _unitOfWork.RestaurantSupplyWithdrawRepository.Add(withdraw);
            await _unitOfWork.CompleteAsync();

            decimal? remainingWeight = null;
            decimal? withdrawnWeight = null;

            if (rawMaterial.UnitWeightInKg.HasValue)
            {
                remainingWeight = inventoryItem.Quantity * rawMaterial.UnitWeightInKg.Value;
                withdrawnWeight = quantityToDeduct * rawMaterial.UnitWeightInKg.Value;
            }

            var responseData = new
            {
                RemainingQuantity = inventoryItem.Quantity,
                WithdrawnQuantity = quantityToDeduct,
                RemainingWeight = remainingWeight,
                WithdrawnWeight = withdrawnWeight,
                HandlingType = rawMaterial.HandlingType.ToString(),
                IsWeightBased = rawMaterial.IsWeightBased
            };

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم السحب بنجاح",
                Data = responseData
            };
        }
        public async Task<Response> GetWithdrawHistoryAsync(int rawMaterialId)
        {
            var rawMaterial = await _unitOfWork.RawMaterialRepository.GetAsync(rawMaterialId);
            if (rawMaterial == null || !rawMaterial.IsRestaurantSupply)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المنتج غير موجود أو ليس منتج صالة"
                };

            var spec = new BaseSpecification<RestaurantSupplyWithdraw>(x => x.RawMaterialId == rawMaterialId);
            spec.AddOrderByDesc(x => x.WithdrawDate);

            var data = await _unitOfWork.RestaurantSupplyWithdrawRepository.GetAllWithSpecAsync(spec);
            var result = data.Adapt<List<RestaurantSupplyWithdrawDetailsDto>>();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استرجاع سجل السحب بنجاح",
                Data = result
            };
        }

        public async Task<Response> GetAllWithdrawHistoryAsync(PaginationSpecParams specParams)
        {
            var spec = new BaseSpecification<RestaurantSupplyWithdraw>();
            spec.AddOrderByDesc(x => x.WithdrawDate);
            spec.ApplyPagination(
                specParams.PageSize * (specParams.PageIndex - 1),
                specParams.PageSize
            );

            var data = await _unitOfWork.RestaurantSupplyWithdrawRepository.GetAllWithSpecAsync(spec);
            var count = await _unitOfWork.RestaurantSupplyWithdrawRepository.GetCountWithSpecAsync(spec);

            var result = data.Adapt<List<RestaurantSupplyWithdrawDetailsDto>>();
            var pagination = new Pagination(specParams.PageSize, specParams.PageIndex, count, result);

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استرجاع سجل السحب بنجاح",
                Data = pagination
            };
        }

        private async Task<decimal> GetTotalWithdrawnAsync(int rawMaterialId)
        {
            var spec = new BaseSpecification<RestaurantSupplyWithdraw>(x => x.RawMaterialId == rawMaterialId);
            var withdraws = await _unitOfWork.RestaurantSupplyWithdrawRepository.GetAllWithSpecAsync(spec);
            return withdraws.Sum(x => x.Quantity);
        }
    }
}