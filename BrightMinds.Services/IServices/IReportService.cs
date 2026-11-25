
namespace BrightMinds.Services.IServices
{
    public interface IReportService
    {
        public Task<Response> GenerateRawMaterialReport(int rawMaterialId);
        public Task<Response> GenerateShiftReport(int shiftId);
    }
}
