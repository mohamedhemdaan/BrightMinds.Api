using BrightMinds.Core.Enums;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;
using BrightMinds.Core.ExtensionHelpers;
using BrightMinds.Core.Specifications;
using BrightMinds.Services.Dtos.ShiftDtos;
using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Library.Services.Dtos;
using Mapster;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Security.Claims;

namespace BrightMinds.Services.Services
{
    public class ShiftService : IShiftService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor; 

        public ShiftService(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Response> CreateShift()
        {
            var claims = _contextAccessor.HttpContext?.User?.Claims;
            if (claims == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = "لازم تسجل دخول الاول"
                };

            var firstName = claims?.FirstOrDefault(c => c.Type == "FirstName")?.Value;
            var lastName = claims?.FirstOrDefault(c => c.Type == "LastName")?.Value;
            var fullName = $"{firstName} {lastName}".Trim();

            var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            var lastShift = await _unitOfWork.ShiftRepository.GetLatestShiftForUser(userId);
            if (lastShift != null && lastShift.Status == ShiftStatus.Opened)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "لازم تقفل اخر شيفت ليك، لو مش عارف تقفله كلم مسؤول النظام"
                };

            var shift = new Shift()
            {
                UserId = userId,
                EmployeeName = fullName
            };

            await _unitOfWork.ShiftRepository.Add(shift);
            await _unitOfWork.CompleteAsync();

            var remainingItems = await GetRemainingItemsFromLastClosedShift();
            if (remainingItems.Any())
            {
                var salesWarehouse = await _unitOfWork.WarehouseRepository.GetByTypeAsync(WarehouseType.SalesWarehouse);
                if (salesWarehouse == null)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = "مخزن البيع غير موجود، لا يمكن ترحيل المتبقي"
                    };

                var withdrawHistory = new WithdrawHistory
                {
                    WarehouseId = salesWarehouse.Id,
                    RecipientName = fullName,
                    UserId = userId,
                    ChefShiftId = shift.Id,       
                    Date = DateTime.UtcNow,
                    NumberOfItems = remainingItems.Count,
                    WithdrawItems = remainingItems
                };

                await _unitOfWork.WithdrawHistoryRepository.Add(withdrawHistory);
            }
            await _unitOfWork.CompleteAsync();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم بدأ الشيفت بنجاح",
                Data = new
                {
                    Id = shift.Id
                }
            };
        }

        public async Task<Response> CloseShift()
        {
            var shift = _contextAccessor.HttpContext.GetActiveShift();
            if (shift == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "لازم تبدأ شيفت الاول"
                };

            var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = "سجل دخول الاول"
                };

            if (shift.UserId != userId)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = "الموظف الى بدأ الشيفت لازم هو الي ينهيه، لو بتواجهك اى مشكلة كلم مدير النظام"
                };

            if (shift.Status == ShiftStatus.ForcedClosedByAdmin)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "الشيفت تم غلقه بالفعل من قبل مسؤول النظام"
                };
            else if (shift.Status == ShiftStatus.Closed)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "تم غلق الشيفت بالفعل"
                };

            var histories = await _unitOfWork.WithdrawHistoryRepository
                .GetAllWithSpecAsync(new WithdrawHistorySpecifications(shift));

            var allItems = histories.SelectMany(h => h.WithdrawItems).ToList();
            var unrecordedItems = allItems.Where(i => !i.ConsumedQuantity.HasValue).ToList();

            if (unrecordedItems.Any())
            {
                var itemNames = string.Join("، ", unrecordedItems.Select(i => $"{i.Name} (الكمية: {i.Quantity})"));
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = $"لا يمكن إغلاق الشيفت قبل تسجيل الاستهلاك للعناصر التالية: {itemNames}"
                };
            }

            shift.EndTimeStamp = DateTime.UtcNow;
            shift.Status = ShiftStatus.Closed;
            shift.TotalHours = (shift.EndTimeStamp - shift.StartTimeStamp).Value.TotalHours;
            
            _unitOfWork.ShiftRepository.Update(shift);
            await _unitOfWork.CompleteAsync();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم غلق الشيفت بنجاح"
            };
        }

        public async Task<Response> GetAllShifts(ShiftSpecParams specParams)
        {
            var spec = new ShiftSpecifications(specParams, false);
            var data = await _unitOfWork.ShiftRepository.GetAllWithSpecAsync(spec);

            var countSpec = new ShiftSpecifications(specParams, true);
            var count = await _unitOfWork.ShiftRepository.GetCountWithSpecAsync(countSpec);

            var result = data.Adapt<List<ShiftDetailsDto>>();
            var pagination = new Pagination(specParams.PageSize, specParams.PageIndex, count, result);

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استعادة بيانات الشيفتات بنجاح",
                Data = pagination
            };
        }

        public async Task<Response> GetShiftById(int id)
        {
            var shift = await _unitOfWork.ShiftRepository.GetAsync(id);
            if (shift == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "الشيفت مش موجود"
                };

            var result = shift.Adapt<ShiftDetailsDto>();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استرجاع بيانات الشيفت بنجاح",
                Data = result
            };
        }

        public async Task<Response> ForceCloseShift(int id)
        {
            var shift = await _unitOfWork.ShiftRepository.GetAsync(id);
            if (shift == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "الشيفت مش موجود"
                };

            if (shift.Status == ShiftStatus.Closed || shift.Status == ShiftStatus.ForcedClosedByAdmin)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "تم غلق الشيفت بالفعل"
                };

            shift.EndTimeStamp = DateTime.UtcNow;
            shift.Status = ShiftStatus.ForcedClosedByAdmin;
            shift.TotalHours = (shift.EndTimeStamp - shift.StartTimeStamp).Value.TotalHours;

            _unitOfWork.ShiftRepository.Update(shift);
            await _unitOfWork.CompleteAsync();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم غلق الشيفت بنجاح"
            };

        }

        public async Task<Response> GetLastestShift()
        {
            if (_contextAccessor.HttpContext?.User == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = "Unauthorized User"
                };

            var userId = _contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المستخدم مش موجود"
                };

            var latestShift = await _unitOfWork.ShiftRepository.GetLatestShiftForUser(userId);
            if (latestShift == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المستخدم معملش اى شيفت قبل كدا"
                };

            var result = latestShift.Adapt<ShiftDetailsDto>();
            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استعادة بيانات احدث شيفت بنجاح",
                Data = result
            };
        }

        private async Task<List<WithdrawItem>> GetRemainingItemsFromLastClosedShift()
        {
            var lastClosedShift = await _unitOfWork.ShiftRepository
                .GetAllWithSpecAsync(new BaseSpecification<Shift>(x =>
                    (x.Status == ShiftStatus.Closed || x.Status == ShiftStatus.ForcedClosedByAdmin) &&
                    x.EndTimeStamp != null)
                );

            var ordered = lastClosedShift.OrderByDescending(x => x.EndTimeStamp).FirstOrDefault();
            if (ordered == null)
                return new List<WithdrawItem>();

            var withdrawHistories = await _unitOfWork.WithdrawHistoryRepository
                .GetAllWithSpecAsync(new WithdrawHistorySpecifications(ordered));

            var remainingItems = new List<WithdrawItem>();
            foreach (var wh in withdrawHistories)
            {
                foreach (var item in wh.WithdrawItems)
                {
                    if (item.ConsumedQuantity.HasValue && item.ConsumedQuantity < item.Quantity)
                    {
                        var remainingQuantity = item.Quantity - item.ConsumedQuantity.Value;
                        var newItem = new WithdrawItem
                        {
                            Name = item.Name,
                            Quantity = remainingQuantity,
                            ContainerQuantity = item.ContainerQuantity,
                            UnitType = item.UnitType,
                            DisplayDetails = item.DisplayDetails + " (متبقي من شيفت سابق)",
                            ConsumedQuantity = 0,        
                            SourceWithdrawItemId = item.Id
                        };
                        remainingItems.Add(newItem);
                    }
                }
            }
            return remainingItems;
        }

        public async Task<Response> RecordConsumption(int shiftId, List<ConsumptionItemDto> consumptionList)
        {
            var shift = await _unitOfWork.ShiftRepository.GetAsync(shiftId);
            if (shift == null)
                return new Response() { StatusCode = 404, Message = "الشيفت غير موجود" };

            var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (shift.UserId != userId)
                return new Response() { StatusCode = 401, Message = "غير مصرح لك بتعديل هذا الشيفت" };

            if (shift.Status != ShiftStatus.Opened)
                return new Response() { StatusCode = 400, Message = "لا يمكن تعديل استهلاك شيفت مغلق" };

            var histories = await _unitOfWork.WithdrawHistoryRepository
                .GetAllWithSpecAsync(new WithdrawHistorySpecifications(shift));

            var allItems = histories.SelectMany(h => h.WithdrawItems).ToList();

            foreach (var cons in consumptionList)
            {
                var item = allItems.FirstOrDefault(i => i.Id == cons.WithdrawItemId);
                if (item == null)
                    return new Response() { StatusCode = 404, Message = $"العنصر {cons.WithdrawItemId} غير موجود في هذا الشيفت" };

                if (cons.ConsumedQuantity > item.Quantity)
                    return new Response() { StatusCode = 400, Message = $"الكمية المستهلكة لا يمكن أن تتجاوز الكمية الأصلية للعنصر {item.Name}" };

                item.ConsumedQuantity = cons.ConsumedQuantity;
                _unitOfWork.WithdrawHistoryRepository.Update(item.WithdrawHistory);    
            }

            await _unitOfWork.CompleteAsync();
            return new Response() { StatusCode = 200, Message = "تم تسجيل الاستهلاك بنجاح" };
        }
    }
}
