using BrightMinds.Core.Enums;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;
using BrightMinds.Core.Specifications;
using BrightMinds.Services.Dtos.SalesWarehouseWithdrawRequestDtos;
using BrightMinds.Services.Hubs;
using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Library.Services.Dtos;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.Net;
using System.Security.Claims;
using BrightMinds.Core.ExtensionHelpers;
using Microsoft.AspNetCore.Identity;

namespace BrightMinds.Services.Services
{
    public class SalesWarehouseWithdrawRequestService : ISalesWarehouseWithdrawRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly UserManager<AppUser> _userManager;

        public SalesWarehouseWithdrawRequestService(
            IUnitOfWork unitOfWork, 
            IHttpContextAccessor contextAccessor, 
            IHubContext<NotificationHub> hubContext,
            UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _hubContext = hubContext;
            _userManager = userManager;
        }

        public async Task<Response> Create(SalesWarehouseWithdrawRequestDto requestDto)
        {
            foreach(var item in requestDto.RequestedItems)
            {
                var inventoryItem = await _unitOfWork.WarehouseProductsInventoryRepository.GetWithSpecAsync(new IncludeProductDetailsSpecifications(item.InventoryItemId));
                if (inventoryItem == null)
                {
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = $"المنتج المرتبط بالمعرف {item.InventoryItemId} مش موجود"
                    };
                }
            }

            var request = requestDto.Adapt<SalesWarehouseWithdrawRequest>();
            request.SenderId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            request.Status = RequestStatus.Pending;

            var activeShift = _contextAccessor.HttpContext?.GetRequiredActiveShift();
            request.ShiftId = activeShift.Id;

            foreach (var item in request.RequestedItems)
            {
                var inventoryItem = await _unitOfWork.WarehouseProductsInventoryRepository.GetWithSpecAsync(new IncludeProductDetailsSpecifications(item.InventoryItemId));
                item.ProductPortionId = inventoryItem.ProductPortionId;
            }

            await _unitOfWork.SalesWarehouseWithdrawRequestRepository.Add(request);
            await _unitOfWork.CompleteAsync();

            var notification = request.Adapt<SalesWarehouseWithdrawRequestDetailsDto>();
            await _hubContext.Clients.All.SendAsync("ReceiveWithdrawRequest", notification);

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم ارسال طلب السحب بنجاح"
            };
        }

        public async Task<Response> GetAll(SalesWarehouseWithdrawRequestSpec specParams)
        {
            var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = "User Unauthorized"
                };

            SalesWarehouseWithdrawrequestSpecifications spec;
            SalesWarehouseWithdrawrequestSpecifications countSpec;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "الموظف مش موجود"
                };

            if(await _userManager.IsInRoleAsync(user, UserRole.MorningShiftChef.ToString()) ||
               await _userManager.IsInRoleAsync(user, UserRole.NightShiftChef.ToString()))
            {
                spec = new SalesWarehouseWithdrawrequestSpecifications(specParams, false, userId, null);
                countSpec = new SalesWarehouseWithdrawrequestSpecifications(specParams, true, userId, null);
            }
            else
            {
                spec = new SalesWarehouseWithdrawrequestSpecifications(specParams, false);
                countSpec = new SalesWarehouseWithdrawrequestSpecifications(specParams, true);
            }

            var data = await _unitOfWork.SalesWarehouseWithdrawRequestRepository.GetAllWithSpecAsync(spec);

            var count = await _unitOfWork.SalesWarehouseWithdrawRequestRepository.GetCountWithSpecAsync(countSpec);

            var result = data.Adapt<List<SalesWarehouseWithdrawRequestDetailsDto>>();
            var pagination = new Pagination(specParams.PageSize, specParams.PageIndex, count, result);

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استعادة بيانات الطلبات بنجاح",
                Data = pagination
            };
        }

        public async Task<Response> AcceptRequest(int requestId)
        {
            var request = await _unitOfWork.SalesWarehouseWithdrawRequestRepository.GetAsync(requestId);
            if (request == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "الطلب مش موجود"
                };

            if (request.Status == RequestStatus.Accepted)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "الطلب اتقبل بالفعل"
                };

            if (request.Status == RequestStatus.Rejected)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "تم رفض الطلب من قبل"
                };

            request.Status = RequestStatus.Accepted;
            _unitOfWork.SalesWarehouseWithdrawRequestRepository.Update(request);
            await _unitOfWork.CompleteAsync();

                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "تم قبول الطلب"
                };
        }

        public async Task<Response> RejectRequest(int requestId)
        {
            var request = await _unitOfWork.SalesWarehouseWithdrawRequestRepository.GetAsync(requestId);
            if (request == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "الطلب مش موجود"
                };

            if (request.Status == RequestStatus.Accepted)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "الطلب اتقبل بالفعل"
                };

            if (request.Status == RequestStatus.Rejected)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "تم رفض الطلب من قبل"
                };

            request.Status = RequestStatus.Rejected;
            _unitOfWork.SalesWarehouseWithdrawRequestRepository.Update(request);
            await _unitOfWork.CompleteAsync();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم رفض الطلب"
            };

        }
    }
}
