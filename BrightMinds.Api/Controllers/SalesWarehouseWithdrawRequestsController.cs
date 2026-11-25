using BrightMinds.Services.Dtos.SalesWarehouseWithdrawRequestDtos;
using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightMinds.Api.Controllers
{
    public class SalesWarehouseWithdrawRequestsController : ApiBaseController
    {
        private readonly ISalesWarehouseWithdrawRequestService _requestService;
        public SalesWarehouseWithdrawRequestsController(ISalesWarehouseWithdrawRequestService requestService)
        {
            _requestService = requestService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,NightShiftChef,MorningShiftChef")]
        public async Task<IActionResult> Create(SalesWarehouseWithdrawRequestDto requestDto)
        {
            var result = await _requestService.Create(requestDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,NightShiftChef,MorningShiftChef,MorningShiftWarehouseClerk,NightShiftWarehouseClerk,WarehouseManager")]
        public async Task<IActionResult> GetAll([FromQuery]SalesWarehouseWithdrawRequestSpec specParams)
        {
            var result = await _requestService.GetAll(specParams);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{requestId}/Accept")]
        [Authorize(Roles = "Admin,MorningShiftWarehouseClerk,NightShiftWarehouseClerk,WarehouseManager")]
        public async Task<IActionResult> Accept(int requestId)
        {
            var result = await _requestService.AcceptRequest(requestId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{requestId}/Reject")]
        [Authorize(Roles = "Admin,MorningShiftWarehouseClerk,NightShiftWarehouseClerk,WarehouseManager")]
        public async Task<IActionResult> Reject(int requestId)
        {
            var result = await _requestService.RejectRequest(requestId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
