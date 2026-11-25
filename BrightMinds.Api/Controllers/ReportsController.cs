using BrightMinds.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightMinds.Api.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Authorize]
    public class ReportsController : ApiBaseController
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("RawMaterials/{rawMaterialId}")]
        public async Task<IActionResult> GetRawMaterialReport(int rawMaterialId)
        {
            var result = await _reportService.GenerateRawMaterialReport(rawMaterialId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("Shifts/{shiftId}")]
        public async Task<IActionResult> GetShiftReport(int shiftId)
        {
            var result = await _reportService.GenerateShiftReport(shiftId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
