using BrightMinds.Core.Enums;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;
using BrightMinds.Core.Specifications;
using BrightMinds.Services.Dtos.SupplyDtos;
using BrightMinds.Services.Dtos.SupplyInstallmentDtos;
using BrightMinds.Services.Dtos.SupplyItemDtos;
using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Library.Services.Dtos;
using Mapster;
using System.Net;

namespace BrightMinds.Services.Services
{
    public class SupplyService : ISupplyService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SupplyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private async Task<Response> ValidateSupplyInstallments(Supply supply)
        {
            var supplyInstallmentSum = supply.Installments.Sum(x => x.AmountPaid);
            if (supplyInstallmentSum > supply.TotalCost)
            {
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "مبلغ التقسيطات الكلي اكبر من اجمالي تكلفة التوريد"
                };
            }
            else if (supplyInstallmentSum == supply.TotalCost)
            {
                supply.RemainingAmount = 0;
                supply.Status = SupplyPaymentStatus.Paid;
            }
            else
            {
                supply.RemainingAmount = supply.TotalCost - supplyInstallmentSum;
                supply.Status = SupplyPaymentStatus.PartiallyPaid;
            }

            var result = supply.Adapt<SupplyDetailsDto>();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم تحديث التقسيط بنجاح",
                Data = result
            };
        }
        public async Task<Response> CreateAsync(SupplyDto supplyDto)
        {
            var supplier = await _unitOfWork.SupplierRepository.GetAsync(supplyDto.SupplierId);
            if (supplier == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المُوَرد غير موجود",
                };

            var duplicateGroups = supplyDto.SupplyItems
                .GroupBy(x => new { x.RawMaterialId, x.ReceivedBy })
                .Where(g => g.Count() > 1)
                .ToList();

            if (duplicateGroups.Any())
            {
                var duplicateIds = duplicateGroups.Select(g => g.Key.RawMaterialId).Distinct().ToList();
                var duplicateNames = new List<string>();
                foreach (var id in duplicateIds)
                {
                    var raw = await _unitOfWork.RawMaterialRepository.GetAsync(id);
                    if (raw != null) duplicateNames.Add(raw.Name);
                }
                var errorMsg = duplicateNames.Any()
                    ? $"يوجد تكرار للمواد التالية بنفس طريقة الاستلام: {string.Join("، ", duplicateNames)}. يرجى تعديل الكمية في صف واحد فقط."
                    : "يوجد تكرار في المواد الخام بنفس طريقة الاستلام. يرجى تعديل الكمية في صف واحد فقط.";

                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = errorMsg
                };
            }

            var supply = supplyDto.Adapt<Supply>();
            supply.SupplierName = supplier.Name;
            supply.SupplyItems.Clear();

            decimal totalCost = 0;

            foreach (SupplyItemDto itemDto in supplyDto.SupplyItems)
            {
                var rawMaterial = await _unitOfWork.RawMaterialRepository.GetAsync(itemDto.RawMaterialId);
                if (rawMaterial == null)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = $"المادة الخام {itemDto.RawMaterialId} غير موجودة",
                    };

                SupplyItem supplyItem = null;

                if (rawMaterial.IsMeat)
                {
                    if (!itemDto.ReceivedWeightKg.HasValue || itemDto.ReceivedWeightKg <= 0)
                        return new Response()
                        {
                            StatusCode = (int)HttpStatusCode.BadRequest,
                            Message = $"الوزن الفعلي الكلي مطلوب لمنتجات اللحوم ({rawMaterial.Name})"
                        };

                    if (!itemDto.ExchangePrice.HasValue || !itemDto.PriceMultiplier.HasValue)
                        return new Response()
                        {
                            StatusCode = (int)HttpStatusCode.BadRequest,
                            Message = $"سعر البورصة والمعامل مطلوبان لمنتجات اللحوم ({rawMaterial.Name})"
                        };

                    var pricePerKg = itemDto.ExchangePrice.Value * itemDto.PriceMultiplier.Value;
                    var totalWeight = itemDto.ReceivedWeightKg.Value;
                    var itemTotalCost = totalWeight * pricePerKg;
                    var pricePerUnit = itemDto.ContainerQuantity > 0 ? itemTotalCost / itemDto.ContainerQuantity.Value : 0;

                    var totalSubUnits = itemDto.ContainerQuantity.Value * rawMaterial.ContainerSize;

                    supplyItem = new SupplyItem
                    {
                        Name = rawMaterial.Name,
                        ContainerQuantity = itemDto.ContainerQuantity,
                        Quantity = totalSubUnits,
                        UnitPrice = pricePerUnit,
                        UnitType = rawMaterial.UnitType,
                        RawMaterialId = rawMaterial.Id,
                        DisplayDetails = $"{itemDto.ContainerQuantity} {rawMaterial.ContainerUnitName} (وزن: {totalWeight:F2} كجم) = {totalSubUnits} {rawMaterial.SubUnitName}",
                        ReceivedWeightKg = itemDto.ReceivedWeightKg,
                        ExchangePrice = itemDto.ExchangePrice,
                        PriceMultiplier = itemDto.PriceMultiplier,
                        ReceivedBy = itemDto.ReceivedBy,
                        OriginalQuantity = itemDto.ContainerQuantity,
                        OriginalPrice = itemDto.ContainerPrice
                    };

                    totalCost += itemTotalCost;
                }
                else
                {
                    decimal containerQuantity;
                    decimal containerPrice;
                    decimal totalQuantity;
                    decimal unitPriceCalc;
                    decimal originalQuantity;
                    decimal originalPrice;

                    if (itemDto.ReceivedBy == ReceivingMethod.Container)
                    {
                        if (!itemDto.ContainerQuantity.HasValue || itemDto.ContainerQuantity <= 0)
                            return new Response()
                            {
                                StatusCode = (int)HttpStatusCode.BadRequest,
                                Message = $"عدد الحاويات مطلوب ({rawMaterial.Name})"
                            };

                        if (!itemDto.ContainerPrice.HasValue || itemDto.ContainerPrice <= 0)
                            return new Response()
                            {
                                StatusCode = (int)HttpStatusCode.BadRequest,
                                Message = $"سعر الحاوية مطلوب ({rawMaterial.Name})"
                            };

                        containerQuantity = itemDto.ContainerQuantity.Value;
                        containerPrice = itemDto.ContainerPrice.Value;
                        totalQuantity = containerQuantity * rawMaterial.ContainerSize;
                        unitPriceCalc = containerPrice / rawMaterial.ContainerSize;

                        originalQuantity = containerQuantity;
                        originalPrice = containerPrice;
                    }
                    else
                    {
                        if (!itemDto.ReceivedQuantity.HasValue || itemDto.ReceivedQuantity <= 0)
                            return new Response()
                            {
                                StatusCode = (int)HttpStatusCode.BadRequest,
                                Message = $"الكمية مطلوبة ({rawMaterial.Name})"
                            };

                        if (!itemDto.UnitPrice.HasValue || itemDto.UnitPrice <= 0)
                            return new Response()
                            {
                                StatusCode = (int)HttpStatusCode.BadRequest,
                                Message = $"سعر الوحدة مطلوب ({rawMaterial.Name})"
                            };

                        totalQuantity = itemDto.ReceivedQuantity.Value;
                        unitPriceCalc = itemDto.UnitPrice.Value;
                        containerQuantity = totalQuantity / rawMaterial.ContainerSize;
                        containerPrice = unitPriceCalc * rawMaterial.ContainerSize;

                        originalQuantity = totalQuantity;
                        originalPrice = unitPriceCalc;
                    }

                    supplyItem = new SupplyItem
                    {
                        Name = rawMaterial.Name,
                        ContainerQuantity = containerQuantity,
                        Quantity = totalQuantity,
                        UnitPrice = unitPriceCalc,
                        UnitType = rawMaterial.UnitType,
                        RawMaterialId = rawMaterial.Id,
                        ReceivedBy = itemDto.ReceivedBy,
                        OriginalQuantity = originalQuantity,
                        OriginalPrice = originalPrice,
                        DisplayDetails = itemDto.ReceivedBy == ReceivingMethod.Container
                            ? $"{containerQuantity} {rawMaterial.ContainerUnitName} = {totalQuantity} {rawMaterial.SubUnitName ?? rawMaterial.UnitType.ToString()}"
                            : $"{totalQuantity} {rawMaterial.SubUnitName ?? rawMaterial.UnitType.ToString()}"
                    };

                    if (rawMaterial.TotalContainerPrice != containerPrice)
                    {
                        rawMaterial.TotalContainerPrice = containerPrice;
                        _unitOfWork.RawMaterialRepository.Update(rawMaterial);
                    }

                    totalCost += totalQuantity * unitPriceCalc;
                }

                supply.SupplyItems.Add(supplyItem);
            }

            supply.TotalCost = totalCost;

            if (supply.PaidAmount == 0)
            {
                supply.Status = SupplyPaymentStatus.NotPaid;
                supply.RemainingAmount = supply.TotalCost;
            }
            else if (supply.PaidAmount > supply.TotalCost)
            {
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "المدفوعات تتجاوز تكلفة التوريد"
                };
            }
            else if (supply.PaidAmount == supply.TotalCost)
            {
                supply.Status = SupplyPaymentStatus.Paid;
                supply.RemainingAmount = 0;
            }
            else
            {
                supply.Status = SupplyPaymentStatus.PartiallyPaid;
                supply.RemainingAmount = supply.TotalCost - supply.PaidAmount;
            }

            if (supply.PaidAmount > 0)
            {
                supply.Installments.Add(new SupplyInstallment()
                {
                    Supply = supply,
                    AmountPaid = supply.PaidAmount,
                    PaymentDate = DateTime.UtcNow
                });
            }

            await _unitOfWork.SupplyRepository.Add(supply);
            await _unitOfWork.CompleteAsync();

            var mainWarehouse = await _unitOfWork.WarehouseRepository.GetByTypeAsync(WarehouseType.MainWarehouse);
            if (mainWarehouse == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المخزن الرئيسي غير موجود",
                };

            var inventoryItems = new List<WarehouseSuppliesInventory>();
            foreach (var item in supply.SupplyItems)
            {
                var rawMaterial = await _unitOfWork.RawMaterialRepository.GetAsync(item.RawMaterialId);
                var inventoryItem = new WarehouseSuppliesInventory
                {
                    WarehouseId = mainWarehouse.Id,
                    SupplyItemId = item.Id,
                    ContainerQuantity = item.ContainerQuantity,
                    Quantity = item.Quantity,
                    Price = item.UnitPrice,
                    UnitType = item.UnitType,
                    DisplayDetails = item.DisplayDetails
                };
                inventoryItems.Add(inventoryItem);
            }

            await _unitOfWork.WarehouseSuppliesInventoryRepository.AddInventoryItems(inventoryItems);
            await _unitOfWork.CompleteAsync();

            SupplyDetailsDto supplyDetailsDto = supply.Adapt<SupplyDetailsDto>();
            return new Response()
            {
                StatusCode = (int)HttpStatusCode.Created,
                Message = "تم إنشاء التوريد بنجاح",
                Data = supplyDetailsDto
            };
        }

        public async Task<Response> UpdateAsync(SupplyDto supplyDto, int id)
        {
            var supply = await _unitOfWork.SupplyRepository.GetWithSpecAsync(new SupplyWithItemsSpecification(id));
            if (supply == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "التوريد غير موجود"
                };

            var supplier = await _unitOfWork.SupplierRepository.GetAsync(supplyDto.SupplierId);
            if (supplier == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المورد غير موجود"
                };

            var duplicateGroups = supplyDto.SupplyItems
                .GroupBy(x => new { x.RawMaterialId, x.ReceivedBy })
                .Where(g => g.Count() > 1)
                .ToList();

            if (duplicateGroups.Any())
            {
                var duplicateIds = duplicateGroups.Select(g => g.Key.RawMaterialId).Distinct().ToList();
                var duplicateNames = new List<string>();
                foreach (var idTemp in duplicateIds)
                {
                    var raw = await _unitOfWork.RawMaterialRepository.GetAsync(idTemp);
                    if (raw != null) duplicateNames.Add(raw.Name);
                }
                var errorMsg = duplicateNames.Any()
                    ? $"يوجد تكرار للمواد التالية بنفس طريقة الاستلام: {string.Join("، ", duplicateNames)}. يرجى تعديل الكمية في صف واحد فقط."
                    : "يوجد تكرار في المواد الخام بنفس طريقة الاستلام. يرجى تعديل الكمية في صف واحد فقط.";

                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = errorMsg
                };
            }

            supply.SupplyItems.Clear();
            decimal totalCost = 0;

            foreach (SupplyItemDto itemDto in supplyDto.SupplyItems)
            {
                var rawMaterial = await _unitOfWork.RawMaterialRepository.GetAsync(itemDto.RawMaterialId);
                if (rawMaterial == null)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = $"المادة الخام {itemDto.RawMaterialId} غير موجودة"
                    };

                SupplyItem supplyItem = null;

                if (rawMaterial.IsMeat)
                {
                    if (!itemDto.ReceivedWeightKg.HasValue || itemDto.ReceivedWeightKg <= 0)
                        return new Response()
                        {
                            StatusCode = (int)HttpStatusCode.BadRequest,
                            Message = $"الوزن الفعلي الكلي مطلوب لمنتجات اللحوم ({rawMaterial.Name})"
                        };

                    if (!itemDto.ExchangePrice.HasValue || !itemDto.PriceMultiplier.HasValue)
                        return new Response()
                        {
                            StatusCode = (int)HttpStatusCode.BadRequest,
                            Message = $"سعر البورصة والمعامل مطلوبان لمنتجات اللحوم ({rawMaterial.Name})"
                        };

                    var pricePerKg = itemDto.ExchangePrice.Value * itemDto.PriceMultiplier.Value;
                    var totalWeight = itemDto.ReceivedWeightKg.Value;
                    var itemTotalCost = totalWeight * pricePerKg;
                    var pricePerUnit = itemDto.ContainerQuantity > 0 ? itemTotalCost / itemDto.ContainerQuantity.Value : 0;

                    var totalSubUnits = itemDto.ContainerQuantity.Value * rawMaterial.ContainerSize;

                    supplyItem = new SupplyItem
                    {
                        Name = rawMaterial.Name,
                        ContainerQuantity = itemDto.ContainerQuantity,
                        Quantity = totalSubUnits,
                        UnitPrice = pricePerUnit,
                        UnitType = rawMaterial.UnitType,
                        RawMaterialId = rawMaterial.Id,
                        SupplyId = id,
                        DisplayDetails = $"{itemDto.ContainerQuantity} {rawMaterial.ContainerUnitName} (وزن: {totalWeight:F2} كجم) = {totalSubUnits} {rawMaterial.SubUnitName}",
                        ReceivedWeightKg = itemDto.ReceivedWeightKg,
                        ExchangePrice = itemDto.ExchangePrice,
                        PriceMultiplier = itemDto.PriceMultiplier,
                        ReceivedBy = itemDto.ReceivedBy,
                        OriginalQuantity = itemDto.ContainerQuantity,
                        OriginalPrice = itemDto.ContainerPrice
                    };

                    totalCost += itemTotalCost;
                }
                else
                {
                    decimal containerQuantity;
                    decimal containerPrice;
                    decimal totalQuantity;
                    decimal unitPriceCalc;
                    decimal originalQuantity;
                    decimal originalPrice;

                    if (itemDto.ReceivedBy == ReceivingMethod.Container)
                    {
                        if (!itemDto.ContainerQuantity.HasValue || itemDto.ContainerQuantity <= 0)
                            return new Response()
                            {
                                StatusCode = (int)HttpStatusCode.BadRequest,
                                Message = $"عدد الحاويات مطلوب ({rawMaterial.Name})"
                            };

                        if (!itemDto.ContainerPrice.HasValue || itemDto.ContainerPrice <= 0)
                            return new Response()
                            {
                                StatusCode = (int)HttpStatusCode.BadRequest,
                                Message = $"سعر الحاوية مطلوب ({rawMaterial.Name})"
                            };

                        containerQuantity = itemDto.ContainerQuantity.Value;
                        containerPrice = itemDto.ContainerPrice.Value;
                        totalQuantity = containerQuantity * rawMaterial.ContainerSize;
                        unitPriceCalc = containerPrice / rawMaterial.ContainerSize;

                        originalQuantity = containerQuantity;
                        originalPrice = containerPrice;
                    }
                    else
                    {
                        if (!itemDto.ReceivedQuantity.HasValue || itemDto.ReceivedQuantity <= 0)
                            return new Response()
                            {
                                StatusCode = (int)HttpStatusCode.BadRequest,
                                Message = $"الكمية مطلوبة ({rawMaterial.Name})"
                            };

                        if (!itemDto.UnitPrice.HasValue || itemDto.UnitPrice <= 0)
                            return new Response()
                            {
                                StatusCode = (int)HttpStatusCode.BadRequest,
                                Message = $"سعر الوحدة مطلوب ({rawMaterial.Name})"
                            };

                        totalQuantity = itemDto.ReceivedQuantity.Value;
                        unitPriceCalc = itemDto.UnitPrice.Value;
                        containerQuantity = totalQuantity / rawMaterial.ContainerSize;
                        containerPrice = unitPriceCalc * rawMaterial.ContainerSize;

                        originalQuantity = totalQuantity;
                        originalPrice = unitPriceCalc;
                    }

                    supplyItem = new SupplyItem
                    {
                        Name = rawMaterial.Name,
                        ContainerQuantity = containerQuantity,
                        Quantity = totalQuantity,
                        UnitPrice = unitPriceCalc,
                        UnitType = rawMaterial.UnitType,
                        RawMaterialId = rawMaterial.Id,
                        SupplyId = id,
                        ReceivedBy = itemDto.ReceivedBy,
                        OriginalQuantity = originalQuantity,
                        OriginalPrice = originalPrice,
                        DisplayDetails = itemDto.ReceivedBy == ReceivingMethod.Container
                            ? $"{containerQuantity} {rawMaterial.ContainerUnitName} = {totalQuantity} {rawMaterial.SubUnitName ?? rawMaterial.UnitType.ToString()}"
                            : $"{totalQuantity} {rawMaterial.SubUnitName ?? rawMaterial.UnitType.ToString()}"
                    };

                    if (rawMaterial.TotalContainerPrice != containerPrice)
                    {
                        rawMaterial.TotalContainerPrice = containerPrice;
                        _unitOfWork.RawMaterialRepository.Update(rawMaterial);
                    }

                    totalCost += totalQuantity * unitPriceCalc;
                }

                supply.SupplyItems.Add(supplyItem);
            }

            supply.SupplierId = supplyDto.SupplierId;
            supply.SupplierName = supplier.Name;
            supply.RecipientName = supplyDto.RecipientName;
            supply.TotalCost = totalCost;

            var totalPaid = supply.Installments.Sum(x => x.AmountPaid);
            supply.PaidAmount = totalPaid;
            supply.RemainingAmount = supply.TotalCost - totalPaid;

            if (supply.RemainingAmount < 0)
            {
                supply.RefundAmount = Math.Abs(supply.RemainingAmount);
                supply.RemainingAmount = 0;
                supply.Status = SupplyPaymentStatus.OverPaid;
            }
            else if (supply.RemainingAmount == 0)
            {
                supply.Status = SupplyPaymentStatus.Paid;
            }
            else if (totalPaid > 0)
            {
                supply.Status = SupplyPaymentStatus.PartiallyPaid;
            }
            else
            {
                supply.Status = SupplyPaymentStatus.NotPaid;
            }

            _unitOfWork.SupplyRepository.Update(supply);
            await _unitOfWork.CompleteAsync();

            await _unitOfWork.WarehouseSuppliesInventoryRepository.DeleteInventoryItemsWithSupplyId(supply.Id);

            var mainWarehouse = await _unitOfWork.WarehouseRepository.GetByTypeAsync(WarehouseType.MainWarehouse);
            if (mainWarehouse == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المخزن الرئيسي غير موجود"
                };

            var inventoryItems = new List<WarehouseSuppliesInventory>();
            foreach (var item in supply.SupplyItems)
            {
                var rawMaterial = await _unitOfWork.RawMaterialRepository.GetAsync(item.RawMaterialId);
                var inventoryItem = new WarehouseSuppliesInventory
                {
                    WarehouseId = mainWarehouse.Id,
                    SupplyItemId = item.Id,
                    ContainerQuantity = item.ContainerQuantity,
                    Quantity = item.Quantity,
                    Price = item.UnitPrice,
                    UnitType = item.UnitType,
                    DisplayDetails = item.DisplayDetails
                };
                inventoryItems.Add(inventoryItem);
            }

            await _unitOfWork.WarehouseSuppliesInventoryRepository.AddInventoryItems(inventoryItems);
            await _unitOfWork.CompleteAsync();

            var result = supply.Adapt<SupplyDetailsDto>();
            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم تحديث التوريد بنجاح",
                Data = result
            };
        }
        public async Task<Response> DeleteAsync(int id)
        {
            var supply = await _unitOfWork.SupplyRepository.GetAsync(id);
            if (supply == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "التوريد مش موجود"
                };

            _unitOfWork.SupplyRepository.Delete(supply);
            await _unitOfWork.CompleteAsync();
            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم حذف التوريد بنجاح"
            };
        }

        public async Task<Response> GetAllAsync(SupplySpecParams specParams)
        {
            var spec = new SupplySpecifications(specParams, isCount: false);
            var data = await _unitOfWork.SupplyRepository.GetAllWithSpecAsync(spec);

            var countSpec = new SupplySpecifications(specParams, isCount: true);
            var count = await _unitOfWork.SupplyRepository.GetCountWithSpecAsync(countSpec);

            var result = data.Adapt<List<SupplyDetailsDto>>();
            var pagination = new Pagination(specParams.PageSize, specParams.PageIndex, count, result);

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استرجاع بيانات كافة التوريدات بنجاح",
                Data = pagination
            };
        }

        public async Task<Response> GetByIdAsync(int id)
        {
            var supply = await _unitOfWork.SupplyRepository.GetWithSpecAsync(new SupplyWithItemsSpecification(id));
            if (supply == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "التوريد مش موجود",
                };
            var result = supply.Adapt<SupplyDetailsDto>();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استرجاع التوريد بنجاح",
                Data = result
            };
        }

        public async Task<Response> CreateSupplyInstallmentAsync(int supplyId, SupplyInstallmentDto supplyInstallmentDto)
        {
            var supply = await _unitOfWork.SupplyRepository.GetWithSpecAsync(new SupplyIncludesSpecifications(supplyId));
            if (supply == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "التوريد مش موجود"
                };

            if (supply.Status == SupplyPaymentStatus.Paid)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "التوريد تم دفعة بالكامل"
                };

            if (supplyInstallmentDto.AmountPaid > supply.RemainingAmount)
            {
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "المبلغ المدفوع اكبر من المبلغ المتبقى لتسديد كامل تكلفة التوريد"
                };
            }
            else if (supplyInstallmentDto.AmountPaid == supply.RemainingAmount)
            {
                supply.Installments.Add(supplyInstallmentDto.Adapt<SupplyInstallment>());
                supply.RemainingAmount = 0;
                supply.PaidAmount += supplyInstallmentDto.AmountPaid;
                supply.Status = SupplyPaymentStatus.Paid;
            }
            else
            {
                supply.Installments.Add(supplyInstallmentDto.Adapt<SupplyInstallment>());
                supply.PaidAmount += supplyInstallmentDto.AmountPaid;
                supply.RemainingAmount = supply.TotalCost - supply.PaidAmount;
                supply.Status = SupplyPaymentStatus.PartiallyPaid;
            }
            await _unitOfWork.CompleteAsync();

            var result = supply.Adapt<SupplyDetailsDto>();
            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم اضافة قسط التوريد بنجاح",
                Data = result
            };
        }

        public async Task<Response> UpdateSupplyInstallmentAsync(int supplyInstalmentId, SupplyInstallmentDto supplyInstallmentDto)
        {
            var supplyInstallment = await _unitOfWork.SupplyInstallmentRepository.GetAsync(supplyInstalmentId);
            if (supplyInstallment == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "القسط مش موجود"
                };

            var supply = await _unitOfWork.SupplyRepository.GetWithSpecAsync(new SupplyIncludesSpecifications(supplyInstallment.SupplyId));
            if (supply == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "التوريد المرتبط بيه القسط مش موجود"
                };

            supplyInstallment.PaymentDate = supplyInstallmentDto.PaymentDate;
            supplyInstallment.SupplyId = supplyInstallmentDto.SupplyId;
            supplyInstallment.AmountPaid = supplyInstallmentDto.AmountPaid;

            var result = await ValidateSupplyInstallments(supply);
            if (result.StatusCode != (int)HttpStatusCode.OK)
                return result;

            await _unitOfWork.CompleteAsync();
            return result;
        }

        public async Task<Response> DeleteSupplyInstallmentAsync(int supplyInstalmentId)
        {
            var supplyInstallment = await _unitOfWork.SupplyInstallmentRepository.GetAsync(supplyInstalmentId);
            if (supplyInstallment == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "قسط التوريد مش موجود"
                };

            var supply = await _unitOfWork.SupplyRepository.GetWithSpecAsync(new SupplyIncludesSpecifications(supplyInstallment.SupplyId));
            if (supply == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "التوريد المرتبط بيه القسط مش موجود"
                };

            _unitOfWork.SupplyInstallmentRepository.Delete(supplyInstallment);
            var calculatedTotalCost = supply.Installments.Where(x => x.Id != supplyInstalmentId).Sum(x => x.AmountPaid);

            if (calculatedTotalCost == 0)
            {
                supply.RemainingAmount = supply.TotalCost;
                supply.Status = SupplyPaymentStatus.NotPaid;
            }
            else
            {
                supply.RemainingAmount = supply.TotalCost - calculatedTotalCost;
                supply.Status = SupplyPaymentStatus.PartiallyPaid;
            }
            await _unitOfWork.CompleteAsync();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم حذف القسط بنجاح"
            };
        }

        public async Task<Response> GetSupplyInstallmentByIdAsync(int supplyInstalmentId)
        {
            var supplyInstallment = await _unitOfWork.SupplyInstallmentRepository.GetAsync(supplyInstalmentId);
            if (supplyInstallment == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "القسط مش موجود"
                };

            var result = supplyInstallment.Adapt<SupplyInstallmentDetailsDto>();
            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استعادة بيانات القسط بنجاح",
                Data = result
            };
        }

        public async Task<Response> GetSupplyInstallmentsAsync(SupplyInstallmentSpecParams specParams)
        {
            var spec = new SupplyInstallmentSpecifications(specParams, false);
            var data = await _unitOfWork.SupplyInstallmentRepository.GetAllWithSpecAsync(spec);

            var countSpec = new SupplyInstallmentSpecifications(specParams, true);
            var count = await _unitOfWork.SupplyInstallmentRepository.GetCountWithSpecAsync(countSpec);

            var result = data.Adapt<List<SupplyInstallmentDetailsDto>>();
            var pagination = new Pagination(specParams.PageSize, specParams.PageIndex, count, result);

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استعادة بيانات الاقساط بنجاح",
                Data = pagination
            };
        }
    }
}
