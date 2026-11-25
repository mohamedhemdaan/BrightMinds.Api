using BrightMinds.Core.Enums;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;
using BrightMinds.Core.Specifications;
using BrightMinds.Services.Dtos.ReportDtos;
using BrightMinds.Services.Dtos.SalesWarehouseWithdrawRequestDtos;
using BrightMinds.Services.Dtos.ShiftDtos;
using BrightMinds.Services.Dtos.WithdrawHistoryDtos;
using BrightMinds.Services.IServices;
using Mapster;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace BrightMinds.Services.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public ReportService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        private async Task<Response> GenerateChefShiftReport(Shift shift)
        {
            var shiftRequests = await _unitOfWork.SalesWarehouseWithdrawRequestRepository.GetAllWithSpecAsync(new SalesWarehouseWithdrawrequestSpecifications(shift));

            var acceptedWithdrawRequestsCount = shiftRequests.Count(x => x.Status == RequestStatus.Accepted);
            var rejectedWithdrawRequestsCount = shiftRequests.Count(x => x.Status == RequestStatus.Rejected);
            var pendingWithdrawRequestsCount = shiftRequests.Count(x => x.Status == RequestStatus.Pending);

            var withdrawHistory = await _unitOfWork.WithdrawHistoryRepository.GetAllWithSpecAsync(new WithdrawHistorySpecifications(shift));

            var productsDuringShift = await _unitOfWork.ProductRepository
                .GetAllWithSpecAsync(new BaseSpecification<Product>(p =>
                    p.CreationDate >= shift.StartTimeStamp &&
                    p.CreationDate <= (shift.EndTimeStamp ?? DateTime.UtcNow) &&
                    p.InitialTotalWeight != null      
                ));

            var consumptionSummary = withdrawHistory
                .SelectMany(wh => wh.WithdrawItems)
                .GroupBy(i => i.Name)
                .Select(g =>
                {
                    var product = productsDuringShift
                        .FirstOrDefault(p => p.Name == g.Key &&
                                             p.CreationDate >= shift.StartTimeStamp &&
                                             p.CreationDate <= (shift.EndTimeStamp ?? DateTime.UtcNow));

                    return new ShiftProductConsumptionDto
                    {
                        ProductName = g.Key,
                        RequestedQuantity = g.Sum(i => i.Quantity),
                        ConsumedQuantity = g.Sum(i => i.ConsumedQuantity ?? 0),
                        RemainingQuantity = g.Sum(i => i.Quantity - (i.ConsumedQuantity ?? 0)),
                        UnitType = g.First().UnitType,

                        InitialWeight = product?.InitialTotalWeight,
                        SettlementLoss = product?.SettlementLoss,
                        FinalWeight = product?.SettledWeight,
                        SettledByUser = product?.SettledByUser != null
                ? $"{product.SettledByUser.FirstName} {product.SettledByUser.LastName}"
                : null,
                        SettlementDate = product?.SettlementDate
                    };
                }).ToList();
            var result = new ChefShiftRepostDetailsDto()
            {
                ShiftDetails = shift.Adapt<ShiftDetailsDto>(),
                WithdrawRequests = shiftRequests.Adapt<List<SalesWarehouseWithdrawRequestDetailsDto>>(),
                WithdrawRequestsCount = shiftRequests.Count,
                AcceptedWithdrawRequestsCount = acceptedWithdrawRequestsCount,
                PendingWithdrawRequestsCount = pendingWithdrawRequestsCount,
                RejectedWithdrawRequestsCount = rejectedWithdrawRequestsCount,
                WithdrawHistory = withdrawHistory.Adapt<List<WithdrawHistoryDetailsDto>>(),
                ConsumptionSummary = consumptionSummary      
            };

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استرجاع تقرير الشيفت بنجاح",
                Data = result
            };
        }
        public async Task<Response> GenerateRawMaterialReport(int rawMaterialId)
        {
            var rawMaterial = await _unitOfWork.RawMaterialRepository.GetAsync(rawMaterialId);
            if (rawMaterial == null)
            {
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المادة الخام مش موجودة"
                };
            }

            var mainWarehouse = await _unitOfWork.WarehouseRepository.GetByTypeAsync(WarehouseType.MainWarehouse);
            if (mainWarehouse == null)
            {
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المخزن الرئيسي مش موجود"
                };
            }
            var subWarehouse = await _unitOfWork.WarehouseRepository.GetByTypeAsync(WarehouseType.SubWarehouse);
            if (subWarehouse == null)
            {
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المخزن الفرعي مش موجود"
                };
            }

            var quantityInMainWarehouse = await _unitOfWork.ReportRepository.GetRawMaterialQuantityInWarehouse(mainWarehouse.Id, rawMaterialId);
            var quantityInSubWarehouse = await _unitOfWork.ReportRepository.GetRawMaterialQuantityInWarehouse(subWarehouse.Id, rawMaterialId);

            var result = new RawMaterialReportDetailsDto()
            {
                QuantityInMainWarehouse = quantityInMainWarehouse,
                QuantityInSubWarehouse = quantityInSubWarehouse,
                TotalQuantity = quantityInMainWarehouse + quantityInSubWarehouse,
                UnitType = rawMaterial.UnitType
            };

            var supplies = await _unitOfWork.ReportRepository.GetEverySupplyContainsRawMaterial(rawMaterialId);
            foreach(var supply in supplies)
            {
                var bySupplyData = supply.SupplyItems.Where(x => x.RawMaterialId == rawMaterialId)
                                                       .Select(x => new
                                                       {
                                                           pricePerUnit = x.UnitPrice,
                                                           quantity = x.Quantity,
                                                           supplierName = supply.SupplierName,
                                                           supplyDate = supply.CreationDate,
                                                           totalPrice = x.Quantity * x.UnitPrice,
                                                           unitType = x.UnitType
                                                       
                                                       }).FirstOrDefault();
                if (bySupplyData == null) continue;

                result.InfoPerSupply.Add(new RawMaterialInSupplyDetailsDto()
                {
                    PricePerUnit = bySupplyData.pricePerUnit,
                    Quantity = bySupplyData.quantity,
                    SupplierName = bySupplyData.supplierName,
                    SupplyDate = bySupplyData.supplyDate,
                    TotalPrice = bySupplyData.totalPrice
                });

            }
           
            var bySupplierData = supplies
                .SelectMany(s => s.SupplyItems)
                .Where(si => si.RawMaterialId == rawMaterialId)
                .GroupBy(si => si.Supply.SupplierName)
                .Select(g => new
                {
                    supplierName = g.Key,
                    totalQuantity = g.Sum(si => si.Quantity)
                })
                .ToList();

            foreach(var item in bySupplierData)
            {
                result.InfoPerSupplier.Add(new RawMaterialBySupplierDetailsDto()
                {
                    SupplierName = item.supplierName,
                    TotalQuantitySupplied = item.totalQuantity
                });
            }

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استرجاع تقارير المنتج بنجاح",
                Data = result
            };
        }

        public async Task<Response> GenerateShiftReport(int shiftId)
        {
            var shift = await _unitOfWork.ShiftRepository.GetAsync(shiftId);
            if (shift == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "الشيفت مش موجود"
                };

            var user = await _userManager.FindByIdAsync(shift.UserId);
            if (user == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "الموظف المسؤول عن الشيفت مش موجود"
                };

            if(await _userManager.IsInRoleAsync(user, UserRole.MorningShiftChef.ToString()) ||
               await _userManager.IsInRoleAsync(user, UserRole.NightShiftChef.ToString())  ||
            await _userManager.IsInRoleAsync(user, UserRole.OperationChef.ToString()))
            {
                return await GenerateChefShiftReport(shift);
            }

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "المنصب الى مطلوب له التقرير مش مدعوم حاليا"
            };
        }
    }
}
