using BrightMinds.Services.Dtos.ShiftDtos;
using BrightMinds.Services.SpecificationParams;

namespace BrightMinds.Services.IServices
{
    public interface IShiftService
    {
        public Task<Response> CreateShift();
        public Task<Response> GetShiftById(int id);
        public Task<Response> GetAllShifts(ShiftSpecParams specParams);
        public Task<Response> CloseShift();
        public Task<Response> ForceCloseShift(int id);
        public Task<Response> GetLastestShift();

        public Task<Response> RecordConsumption(int shiftId, List<ConsumptionItemDto> consumptionList);
    }
}
