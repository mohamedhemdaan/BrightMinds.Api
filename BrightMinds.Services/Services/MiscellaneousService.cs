using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;
using BrightMinds.Core.Specifications;
using BrightMinds.Services.Dtos.MiscellaneousDtos;
using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Library.Services.Dtos;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using BrightMinds.Services.Helpers;

namespace BrightMinds.Services.Services
{

    public class MiscellaneousService : IMiscellaneousService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConfiguration _configuration;    

        public MiscellaneousService(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _configuration = configuration;
        }

        private string SaveImage(IFormFile image)
        {
            if (image == null) return null;
            return PictureSettings.UploadFile(image, "misc");   
        }

        private string GetFullImageUrl(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return null;
            var baseUrl = _configuration["ApiBaseUrl"];
            return $"{baseUrl}/{relativePath}";
        }

        public async Task<Response> CreateItemAsync(MiscellaneousItemDto dto)
        {
            var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            var item = dto.Adapt<MiscellaneousItem>();
            item.CreatedBy = userId;
            item.CreatedAt = DateTime.UtcNow;
            item.IsDeleted = false;

            if (dto.Image != null)
                item.ImageUrl = SaveImage(dto.Image);    

            await _unitOfWork.MiscellaneousItemRepository.Add(item);

            var addition = new MiscellaneousAddition
            {
                Item = item,   

                AddedBy = userId ?? "غير معروف",
                Quantity = item.Quantity,
                AdditionDate = DateTime.UtcNow,
                Notes = "إضافة أولية"
            };
            await _unitOfWork.MiscellaneousAdditionRepository.Add(addition);

            await _unitOfWork.CompleteAsync();

            var result = item.Adapt<MiscellaneousItemDetailsDto>();
            result.TotalWithdrawn = 0;
            result.AvailableQuantity = item.Quantity;
            result.TotalAdded = item.Quantity;        

            result.ImageUrl = GetFullImageUrl(item.ImageUrl);

            return new Response()
            {
                StatusCode = 201,
                Message = "تم الإضافة",
                Data = result
            };
        }

        public async Task<Response> UpdateItemAsync(int id, MiscellaneousItemDto dto)
        {
            var item = await _unitOfWork.MiscellaneousItemRepository.GetAsync(id);
            if (item == null || item.IsDeleted)
                return new Response() { StatusCode = 404, Message = "غير موجود" };

            if (dto.Image != null && !string.IsNullOrEmpty(item.ImageUrl))
                PictureSettings.DeleteFile(item.ImageUrl);

            var oldQuantity = item.Quantity;

            item.Name = dto.Name;
            item.Quantity = dto.Quantity;
            item.Price = dto.Price;
            item.ReceivedBy = dto.ReceivedBy;
            item.ReceivingPlace = dto.ReceivingPlace;
            item.ReceivingDate = dto.ReceivingDate;
            item.Notes = dto.Notes;

            if (dto.Image != null)
                item.ImageUrl = SaveImage(dto.Image);

            _unitOfWork.MiscellaneousItemRepository.Update(item);
            await _unitOfWork.CompleteAsync();

            var result = item.Adapt<MiscellaneousItemDetailsDto>();
            result.TotalWithdrawn = await GetTotalWithdrawnAsync(item.Id);
            result.AvailableQuantity = item.Quantity - result.TotalWithdrawn;

            result.TotalAdded = await GetTotalAddedAsync(item.Id);     

            result.ImageUrl = GetFullImageUrl(item.ImageUrl);    

            return new Response()
            {
                StatusCode = 200,
                Message = "تم التعديل",
                Data = result
            };
        }

        public async Task<Response> DeleteItemAsync(int id)
        {
            var item = await _unitOfWork.MiscellaneousItemRepository.GetAsync(id);
            if (item == null || item.IsDeleted)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المنتج غير موجود"
                };

            item.IsDeleted = true;
            _unitOfWork.MiscellaneousItemRepository.Update(item);
            await _unitOfWork.CompleteAsync();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم حذف المنتج بنجاح"
            };
        }

        public async Task<Response> GetAllItemsAsync(MiscellaneousSpecParams specParams)
        {
            var spec = new BaseSpecification<MiscellaneousItem>(x => !x.IsDeleted &&
                (string.IsNullOrEmpty(specParams.SearchName) || x.Name.Contains(specParams.SearchName)));

            if (string.IsNullOrEmpty(specParams.OrderBy) || specParams.OrderBy == "new")
                spec.AddOrderByDesc(x => x.CreatedAt);
            else
                spec.AddOrderBy(x => x.CreatedAt);

            spec.ApplyPagination(
                specParams.PageSize * (specParams.PageIndex - 1),
                specParams.PageSize
            );

            var data = await _unitOfWork.MiscellaneousItemRepository.GetAllWithSpecAsync(spec);
            var count = await _unitOfWork.MiscellaneousItemRepository.GetCountWithSpecAsync(spec);

            var result = new List<MiscellaneousItemDetailsDto>();
            foreach (var item in data)
            {
                var dto = item.Adapt<MiscellaneousItemDetailsDto>();
                dto.ImageUrl = GetFullImageUrl(item.ImageUrl);

                dto.TotalWithdrawn = await GetTotalWithdrawnAsync(item.Id);
                dto.AvailableQuantity = item.Quantity - dto.TotalWithdrawn;
                dto.TotalAdded = await GetTotalAddedAsync(item.Id);     

                result.Add(dto);
            }

            var pagination = new Pagination(specParams.PageSize, specParams.PageIndex, count, result);

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استرجاع المنتجات بنجاح",
                Data = pagination
            };
        }
        public async Task<Response> GetItemByIdAsync(int id)
        {
            var item = await _unitOfWork.MiscellaneousItemRepository.GetAsync(id);
            if (item == null || item.IsDeleted)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المنتج غير موجود"
                };

            var result = item.Adapt<MiscellaneousItemDetailsDto>();
            result.ImageUrl = GetFullImageUrl(item.ImageUrl);

            result.TotalWithdrawn = await GetTotalWithdrawnAsync(item.Id);
            result.AvailableQuantity = item.Quantity - result.TotalWithdrawn;
            result.TotalAdded = await GetTotalAddedAsync(item.Id);     

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استرجاع المنتج بنجاح",
                Data = result
            };
        }

        public async Task<Response> WithdrawItemAsync(MiscellaneousWithdrawDto dto)
        {
            var item = await _unitOfWork.MiscellaneousItemRepository.GetAsync(dto.ItemId);
            if (item == null || item.IsDeleted)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المنتج غير موجود"
                };

            var totalWithdrawn = await GetTotalWithdrawnAsync(item.Id);
            var available = item.Quantity - totalWithdrawn;

            if (dto.Quantity > available)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = $"العدد المطلوب للسحب ({dto.Quantity}) أكبر من الكمية المتاحة ({available})"
                };

            var withdraw = new MiscellaneousWithdraw()
            {
                ItemId = dto.ItemId,
                WithdrawnBy = dto.WithdrawnBy,
                Quantity = dto.Quantity,
                WithdrawDate = DateTime.UtcNow,
                Notes = dto.Notes
            };

            await _unitOfWork.MiscellaneousWithdrawRepository.Add(withdraw);
            await _unitOfWork.CompleteAsync();

            var updatedItem = await _unitOfWork.MiscellaneousItemRepository.GetAsync(dto.ItemId);
            var result = updatedItem.Adapt<MiscellaneousItemDetailsDto>();
            result.TotalWithdrawn = await GetTotalWithdrawnAsync(item.Id);
            result.AvailableQuantity = updatedItem.Quantity - result.TotalWithdrawn;

            result.TotalAdded = await GetTotalAddedAsync(item.Id);

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم سحب المنتج بنجاح",
                Data = result
            };
        }

        public async Task<Response> GetWithdrawHistoryAsync(int itemId)
        {
            var item = await _unitOfWork.MiscellaneousItemRepository.GetAsync(itemId);
            if (item == null || item.IsDeleted)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المنتج غير موجود"
                };

            var spec = new BaseSpecification<MiscellaneousWithdraw>(x => x.ItemId == itemId);
            spec.AddOrderByDesc(x => x.WithdrawDate);

            var data = await _unitOfWork.MiscellaneousWithdrawRepository.GetAllWithSpecAsync(spec);
            var result = data.Adapt<List<MiscellaneousWithdrawDetailsDto>>();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استرجاع سجل السحب بنجاح",
                Data = result
            };
        }

        public async Task<Response> AddStockAsync(MiscellaneousAddStockDto dto)
        {
            var item = await _unitOfWork.MiscellaneousItemRepository.GetAsync(dto.ItemId);
            if (item == null || item.IsDeleted)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المنتج غير موجود"
                };

            item.Quantity += dto.Quantity;

            var addition = new MiscellaneousAddition
            {
                Item = item,
                AddedBy = dto.AddedBy,
                Quantity = dto.Quantity,
                AdditionDate = DateTime.UtcNow,
                Notes = dto.Notes ?? "إضافة مخزون"
            };

            await _unitOfWork.MiscellaneousAdditionRepository.Add(addition);
            _unitOfWork.MiscellaneousItemRepository.Update(item);
            await _unitOfWork.CompleteAsync();

            var result = item.Adapt<MiscellaneousItemDetailsDto>();
            result.TotalWithdrawn = await GetTotalWithdrawnAsync(item.Id);
            result.AvailableQuantity = item.Quantity - result.TotalWithdrawn;
            result.TotalAdded = await GetTotalAddedAsync(item.Id);
            result.ImageUrl = GetFullImageUrl(item.ImageUrl);

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم إضافة الكمية بنجاح",
                Data = result
            };
        }

        public async Task<Response> GetAdditionHistoryAsync(int itemId)
        {
            var item = await _unitOfWork.MiscellaneousItemRepository.GetAsync(itemId);
            if (item == null || item.IsDeleted)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المنتج غير موجود"
                };

            var spec = new BaseSpecification<MiscellaneousAddition>(x => x.ItemId == itemId);
            spec.AddOrderByDesc(x => x.AdditionDate);

            var data = await _unitOfWork.MiscellaneousAdditionRepository.GetAllWithSpecAsync(spec);
            var result = data.Adapt<List<MiscellaneousAdditionDetailsDto>>();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استرجاع سجل الإضافات بنجاح",
                Data = result
            };
        }
        private async Task<int> GetTotalWithdrawnAsync(int itemId)
        {
            var spec = new BaseSpecification<MiscellaneousWithdraw>(x => x.ItemId == itemId);
            var withdraws = await _unitOfWork.MiscellaneousWithdrawRepository.GetAllWithSpecAsync(spec);
            return withdraws.Sum(x => x.Quantity);
        }

        private async Task<int> GetTotalAddedAsync(int itemId)
        {
            var spec = new BaseSpecification<MiscellaneousAddition>(x => x.ItemId == itemId);
            var additions = await _unitOfWork.MiscellaneousAdditionRepository.GetAllWithSpecAsync(spec);
            return additions.Sum(x => x.Quantity);
        }
    }

}
