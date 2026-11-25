using BrightMinds.Services.Dtos.MiscellaneousDtos;
using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightMinds.Api.Controllers
{
    [Authorize(Roles = "Admin,MorningShiftWarehouseClerk,NightShiftWarehouseClerk,WarehouseManager")]
    public class MiscellaneousStoreController : ApiBaseController
    {
        private readonly IMiscellaneousService _miscellaneousService;

        public MiscellaneousStoreController(IMiscellaneousService miscellaneousService)
        {
            _miscellaneousService = miscellaneousService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] MiscellaneousItemDto dto)
        {
            var result = await _miscellaneousService.CreateItemAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] MiscellaneousItemDto dto)
        {
            var result = await _miscellaneousService.UpdateItemAsync(id, dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _miscellaneousService.DeleteItemAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] MiscellaneousSpecParams specParams) 
        {
            var result = await _miscellaneousService.GetAllItemsAsync(specParams);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _miscellaneousService.GetItemByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] MiscellaneousWithdrawDto dto)
        {
            var result = await _miscellaneousService.WithdrawItemAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{itemId}/withdraw-history")]
        public async Task<IActionResult> GetWithdrawHistory(int itemId)
        {
            var result = await _miscellaneousService.GetWithdrawHistoryAsync(itemId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{itemId}/addition-history")]
        public async Task<IActionResult> GetAdditionHistory(int itemId)
        {
            var result = await _miscellaneousService.GetAdditionHistoryAsync(itemId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("add-stock")]
        public async Task<IActionResult> AddStock([FromBody] MiscellaneousAddStockDto dto)
        {
            var result = await _miscellaneousService.AddStockAsync(dto);
            return StatusCode(result.StatusCode, result);
        }
    }
}