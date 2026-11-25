
using BrightMinds.Services.Dtos.SalesWarehouseWithdrawRequestDtos;
using BrightMinds.Services.SpecificationParams;

namespace BrightMinds.Services.IServices
{
    public interface ISalesWarehouseWithdrawRequestService
    {
        public Task<Response> Create(SalesWarehouseWithdrawRequestDto requestDto);
        public Task<Response> GetAll(SalesWarehouseWithdrawRequestSpec specParams);
        public Task<Response> AcceptRequest(int requestId);
        public Task<Response> RejectRequest(int requestId);
    }
}
