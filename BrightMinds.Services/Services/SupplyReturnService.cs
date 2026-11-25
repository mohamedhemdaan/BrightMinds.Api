using BrightMinds.Core.Enums;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;
using BrightMinds.Core.Specifications;
using BrightMinds.Services.Dtos.SupplyReturnDtos;
using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Library.Services.Dtos;
using Mapster;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Security.Claims;

namespace BrightMinds.Services.Services
{
    public class SupplyReturnService : ISupplyReturnService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public SupplyReturnService(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        private async Task<SupplyReturn> MapToSupplyReturn(SupplyReturnDto dto)
        {
            var supplyReturn = new SupplyReturn()
            {
                SupplyId = dto.SupplyId
            };

            var claims = _contextAccessor.HttpContext?.User?.Claims;

            var firstName = claims?.FirstOrDefault(c => c.Type == "FirstName")?.Value;
            var lastName = claims?.FirstOrDefault(c => c.Type == "LastName")?.Value;

            supplyReturn.UserId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            supplyReturn.ReturnerName = $"{firstName} {lastName}".Trim();

            foreach (var item in dto.Items)
            {
                var supplyReturnItem = await MapToSupplyReturnItem(item);
                supplyReturnItem.SupplyReturn = supplyReturn;
                supplyReturn.Items.Add(supplyReturnItem);
            }

            return supplyReturn;
        }

        private async Task<SupplyReturnItem> MapToSupplyReturnItem(SupplyReturnItemDto dto)
        {
            var supplyItem = await _unitOfWork.SupplyItemRepository.GetAsync(dto.SupplyItemId);
            if (supplyItem == null)
                throw new Exception("Faild To Map to Supply Return Item!");

            var supplyReturnItem = new SupplyReturnItem()
            {
                Name = supplyItem.Name,
                Quantity = dto.Quantity,
                Price = supplyItem.UnitPrice,
                UnitType = supplyItem.UnitType,
                SupplyItemId = dto.SupplyItemId
            };

            return supplyReturnItem;
        }

        private void ApplyReturnToSupplyFinancials(Supply supply, decimal returnTotal)
        {
            supply.TotalCost -= returnTotal;

            var newRemainingAmount = supply.TotalCost - supply.PaidAmount;

            if (newRemainingAmount >= 0)
            {
                supply.RemainingAmount = newRemainingAmount;
                supply.RefundAmount = 0;
            }
            else
            {
                supply.RemainingAmount = 0;
                supply.RefundAmount = Math.Abs(newRemainingAmount);
            }

            RecalculateSupplyStatus(supply);
        }

        private void RecalculateSupplyStatus(Supply supply)
        {
            supply.Status = supply.RefundAmount > 0 ? SupplyPaymentStatus.OverPaid
                          : supply.RemainingAmount == 0 ? SupplyPaymentStatus.Paid
                          : supply.PaidAmount == 0 ? SupplyPaymentStatus.NotPaid
                                                          : SupplyPaymentStatus.PartiallyPaid;
        }

        public async Task<Response> CreateSupplyReturn(SupplyReturnDto supplyReturnDto)
        {
            var supply = await _unitOfWork.SupplyRepository.GetAsync(supplyReturnDto.SupplyId);
            if(supply == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "التوريد مش موجود"
                };

            supplyReturnDto.Items.GroupBy(x => x.SupplyItemId)
                                 .Select(x => new SupplyReturnItemDto()
                                 {
                                     SupplyItemId = x.Key,
                                     Quantity = x.Sum(x => x.Quantity)
                                 });

            foreach(var item in supplyReturnDto.Items)
            {
                var supplyItem = await _unitOfWork.SupplyItemRepository.GetAsync(item.SupplyItemId);
                if(supplyItem == null)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = $"عنصر التوريد المرتبط بالمعرف {item.SupplyItemId} مش موجود"
                    };

                if (item.Quantity > supplyItem.Quantity)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = $"كمية المرتجع من {supplyItem.Name} اكبر من الكمية المُوَردة"
                    };

                var inventoryItem = await _unitOfWork.WarehouseSuppliesInventoryRepository.GetFromMainWarehouseWithSupplyItemId(supplyItem.Id);
                if (inventoryItem == null)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = $"ال {supplyItem.Name} مش موجود فى المخزن الرئيسي"
                    };

                if(item.Quantity > inventoryItem.Quantity)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = $"كمية ال {supplyItem.Name} فى المخزن الرئيسي غير كافية للمرتجع"
                    };

            }

            var supplyReturn = await MapToSupplyReturn(supplyReturnDto);
            await _unitOfWork.SupplyReturnRepository.Add(supplyReturn);
            await _unitOfWork.CompleteAsync();

            var result = supplyReturn.Adapt<SupplyReturnDetailsDto>();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم انشاء طلب المثرتجع بنجاح",
                Data = result
            };
        }

        public async Task<Response> ApproveSupplyReturn(int id)
        {
            var supplyReturn = await _unitOfWork.SupplyReturnRepository.GetWithSpecAsync(new SupplyReturnSpecifications(id));
            if (supplyReturn == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المرتجع مش موجود"
                };

            var supply = await _unitOfWork.SupplyRepository.GetAsync(supplyReturn.SupplyId);
            if (supply == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "التوريد مش موجود"
                };

            if (supplyReturn.Status != SupplyReturnStatus.Pending)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "تم اتخاذ اجراء بشأن هذا الطلب مسبقا"
                };

            foreach(var item in supplyReturn.Items)
            {
                var supplyItem = await _unitOfWork.SupplyItemRepository.GetAsync(item.SupplyItemId);
                if (supplyItem == null)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = "عنصر التوريد مش موجود"
                    };

                var inventoryItem = await _unitOfWork.WarehouseSuppliesInventoryRepository.GetFromMainWarehouseWithSupplyItemId(item.SupplyItemId);
                if (inventoryItem == null)
                {
                    supplyReturn.Status = SupplyReturnStatus.FaildNotEnoughResources;
                    _unitOfWork.SupplyReturnRepository.Update(supplyReturn);
                    await _unitOfWork.CompleteAsync();

                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = $"ال {item.Name} مش موجود فى المخزن الرئيسي"
                    };
                }

                if (inventoryItem.Quantity < item.Quantity)
                {
                    supplyReturn.Status = SupplyReturnStatus.FaildNotEnoughResources;
                    _unitOfWork.SupplyReturnRepository.Update(supplyReturn);
                    await _unitOfWork.CompleteAsync();

                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = $"كمية ال {item.Name} فى المخزن الرئيسي مش هتكفي"
                    };
                }
            }

            foreach (var item in supplyReturn.Items)
            {
                var inventoryItem = await _unitOfWork.WarehouseSuppliesInventoryRepository
                    .GetFromMainWarehouseWithSupplyItemId(item.SupplyItemId);

                inventoryItem.Quantity -= item.Quantity;

                if (inventoryItem.Quantity == 0)
                    _unitOfWork.WarehouseSuppliesInventoryRepository.Delete(inventoryItem);
                else
                    _unitOfWork.WarehouseSuppliesInventoryRepository.Update(inventoryItem);
            }

            var returnTotal = supplyReturn.Items.Sum(x => x.Quantity * x.Price);
            ApplyReturnToSupplyFinancials(supply, returnTotal);

            supplyReturn.Status = SupplyReturnStatus.Approved;
            supplyReturn.ApprovalDate = DateTime.Now;
            supplyReturn.IsApproved = true;

            await _unitOfWork.CompleteAsync();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم اجراء المرتجع بنجاح"
            };
        }

        public async Task<Response> RejectSupplyReturn(int id)
        {
            var supplyReturn = await _unitOfWork.SupplyReturnRepository.GetAsync(id);
            if (supplyReturn == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المرتجع مش موجود"
                };

            if (supplyReturn.Status != SupplyReturnStatus.Pending)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "تم اتخاذ اجراء بشأن هذا المرتجع من قبل"
                };

            supplyReturn.Status = SupplyReturnStatus.Rejected;
            _unitOfWork.SupplyReturnRepository.Update(supplyReturn);
            await _unitOfWork.CompleteAsync();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم رفض طلب المرتجع"
            };
        }

        public async Task<Response> GetAllSupplyReturns(SupplyReturnSpecParams specParams)
        {
            var spec = new SupplyReturnSpecifications(specParams, false);
            var data = await _unitOfWork.SupplyReturnRepository.GetAllWithSpecAsync(spec);

            var countSpec = new SupplyReturnSpecifications(specParams, true);
            var count = await _unitOfWork.SupplyReturnRepository.GetCountWithSpecAsync(countSpec);

            var result = data.Adapt<List<SupplyReturnDetailsDto>>();
            var pagination = new Pagination(specParams.PageSize, specParams.PageIndex, count, result);

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استعادة جميع المرتجعات بنجاح",
                Data = pagination
            };
        }

        public async Task<Response> GetAllSupplyReturnsForSupply(int supplyId)
        {
            var supply = await _unitOfWork.SupplyRepository.GetAsync(supplyId);
            if (supply == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "التوريد مش موجود"
                };

            var supplyReturns = await _unitOfWork.SupplyReturnRepository.GetAllSupplyReturnsWithSupplyId(supplyId); 
            var result = supplyReturns.Adapt<List<SupplyReturnDetailsDto>>();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استعادة كافة المرتجعات الخاصة بالتوريد بنجاح",
                Data = result
            };
        }

        public async Task<Response> GetSupplyReturnById(int id)
        {
            var supplyReturn = await _unitOfWork.SupplyReturnRepository.GetWithSpecAsync(new SupplyReturnSpecifications(id));
            if (supplyReturn == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المرتجع غير موجود"
                };

            var result = supplyReturn.Adapt<SupplyReturnDetailsDto>();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استرجاع المرتجع بنجاح",
                Data = result
            };
        }

    }
}
