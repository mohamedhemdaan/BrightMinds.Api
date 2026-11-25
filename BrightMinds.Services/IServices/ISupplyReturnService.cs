using BrightMinds.Services.Dtos.SupplyReturnDtos;
using BrightMinds.Services.SpecificationParams;

namespace BrightMinds.Services.IServices
{
    public interface ISupplyReturnService
    {
        public Task<Response> CreateSupplyReturn(SupplyReturnDto  supplyReturnDto);
        public Task<Response> GetSupplyReturnById(int id);
        public Task<Response> GetAllSupplyReturns(SupplyReturnSpecParams specParams);
        public Task<Response> GetAllSupplyReturnsForSupply(int supplyId);
        public Task<Response> ApproveSupplyReturn(int id);
        public Task<Response> RejectSupplyReturn(int id);
    }
}
