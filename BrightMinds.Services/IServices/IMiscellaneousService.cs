using BrightMinds.Services.Dtos.MiscellaneousDtos;
using BrightMinds.Services.SpecificationParams;
using Library.Services.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrightMinds.Services.IServices
{
   
        public interface IMiscellaneousService
        {
            Task<Response> CreateItemAsync(MiscellaneousItemDto dto);
            Task<Response> UpdateItemAsync(int id, MiscellaneousItemDto dto);
            Task<Response> DeleteItemAsync(int id);
            Task<Response> GetAllItemsAsync(MiscellaneousSpecParams specParams);
            Task<Response> GetItemByIdAsync(int id);
            Task<Response> WithdrawItemAsync(MiscellaneousWithdrawDto dto);
            Task<Response> GetWithdrawHistoryAsync(int itemId);

            Task<Response> GetAdditionHistoryAsync(int itemId);

            Task<Response> AddStockAsync(MiscellaneousAddStockDto dto);
    }
    }
