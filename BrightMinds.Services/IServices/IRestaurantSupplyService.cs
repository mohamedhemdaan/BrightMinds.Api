using BrightMinds.Core.ISpecificationParams;
using BrightMinds.Services.Dtos.RestaurantSupplyDtos;
using BrightMinds.Services.SpecificationParams;

namespace BrightMinds.Services.IServices
{
    public interface IRestaurantSupplyService
    {
        Task<Response> GetAllSuppliesAsync(PaginationSpecParams specParams);
        Task<Response> GetSupplyByIdAsync(int id);
        Task<Response>  WithdrawAsync(RestaurantSupplyWithdrawDto dto);
        Task<Response> GetWithdrawHistoryAsync(int rawMaterialId);
        Task<Response> GetAllWithdrawHistoryAsync(PaginationSpecParams specParams);
    }
}