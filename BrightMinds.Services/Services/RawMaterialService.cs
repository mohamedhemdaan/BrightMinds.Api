using BrightMinds.Core.Enums;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;
using BrightMinds.Core.Specifications;
using BrightMinds.Services.Dtos.RawMaterialDtos;
using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Library.Services.Dtos;
using Mapster;
using System.Net;

namespace BrightMinds.Services.Services
{
    public class RawMaterialService : IRawMaterialService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RawMaterialService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response> CreateAsync(RawMaterialDto rawMaterialDto)
        {
            var category = await _unitOfWork.CategoryRepository.GetAsync(rawMaterialDto.CategoryId);
            if (category == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "التصنيف غير موجود"
                };

            if (rawMaterialDto.HandlingType == HandlingType.WeightOnly ||
                rawMaterialDto.HandlingType == HandlingType.CountAndWeight)
            {
                if (!rawMaterialDto.UnitWeightInKg.HasValue || rawMaterialDto.UnitWeightInKg <= 0)
                {
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "وزن الوحدة مطلوب للأصناف الوزنية أو العددية+الوزنية."
                    };
                }
            }
            else if (rawMaterialDto.HandlingType == HandlingType.CountOnly)
            {
                if (string.IsNullOrEmpty(rawMaterialDto.SubUnitName))
                {
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "اسم الوحدة الصغرى مطلوب للأصناف العددية."
                    };
                }
                rawMaterialDto.UnitWeightInKg = null;
            }

            if (!string.IsNullOrEmpty(rawMaterialDto.InnerUnitName) && rawMaterialDto.InnerUnitsPerSubUnit <= 1)
            {
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "لو عايز تضيف وحدة داخلية أصغر، لازم يكون عددها أكبر من 1."
                };
            }

            if (string.IsNullOrEmpty(rawMaterialDto.InnerUnitName) && rawMaterialDto.InnerUnitsPerSubUnit != 1)
            {
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "لازم تحدد اسم الوحدة الصغرى لما تحدد عدد أكبر من 1."
                };
            }

            RawMaterial rawMaterial = rawMaterialDto.Adapt<RawMaterial>();

            rawMaterial.ContainerSize = rawMaterialDto.ContainerSize;
            rawMaterial.TotalContainerPrice = rawMaterialDto.TotalContainerPrice;

            if (rawMaterial.UnitWeightInKg.HasValue && rawMaterial.UnitWeightInKg.Value > 0)
            {
                var totalWeightInKg = rawMaterial.ContainerSize * rawMaterial.UnitWeightInKg.Value;
                rawMaterial.PricePerKg = totalWeightInKg > 0 ? rawMaterial.TotalContainerPrice / totalWeightInKg : 0;
            }
            else
            {
                rawMaterial.PricePerKg = null;
            }

            if (rawMaterial.ContainerSize > 0)
            {
                rawMaterial.PricePerPiece = rawMaterial.TotalContainerPrice / rawMaterial.ContainerSize;
            }
            else
            {
                rawMaterial.PricePerPiece = null;
            }

            var (_, _, pricePerInnerUnitCalc, _) = CalculatePricePerUnitWithDetails(rawMaterial);
            rawMaterial.PricePerInnerUnit = pricePerInnerUnitCalc;

            await _unitOfWork.RawMaterialRepository.Add(rawMaterial);
            await _unitOfWork.CompleteAsync();

            var result = rawMaterial.Adapt<RawMaterialDetailsDto>();
            result.CategoryName = category.Name;
            result.IsWeightBased = rawMaterial.IsWeightBased;
            result.TotalWeightPerContainer = rawMaterial.TotalWeightPerContainer;

            var (pricePerUnit, pricePerKg, pricePerInnerUnit, displayUnit) = CalculatePricePerUnitWithDetails(rawMaterial);
            result.PricePerUnit = pricePerUnit;
            result.PricePerSubUnit = pricePerUnit;              
            result.PricePerKg = pricePerKg;
            result.PricePerInnerUnit = pricePerInnerUnit;     
            result.PriceDisplayUnit = displayUnit;

            result.InnerUnitName = rawMaterial.InnerUnitName;
            result.InnerUnitsPerSubUnit = rawMaterial.InnerUnitsPerSubUnit;
            result.InnerUnitWeightInKg = rawMaterial.InnerUnitWeightInKg;

            result.PricePerPiece = rawMaterial.PricePerPiece;

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.Created,
                Message = "تم إنشاء المادة الخام بنجاح",
                Data = result
            };
        }
        public async Task<Response> UpdateAsync(RawMaterialDto rawMaterialDto, int id)
        {
            var rawMaterial = await _unitOfWork.RawMaterialRepository.GetAsync(id);
            if (rawMaterial == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المادة الخام غير موجودة"
                };

            var category = await _unitOfWork.CategoryRepository.GetAsync(rawMaterialDto.CategoryId);
            if (category == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "التصنيف غير موجود"
                };

            if (rawMaterialDto.HandlingType == HandlingType.WeightOnly ||
                rawMaterialDto.HandlingType == HandlingType.CountAndWeight)
            {
                if (!rawMaterialDto.UnitWeightInKg.HasValue || rawMaterialDto.UnitWeightInKg <= 0)
                {
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "وزن الوحدة مطلوب للأصناف الوزنية أو العددية+الوزنية."
                    };
                }
            }
            else if (rawMaterialDto.HandlingType == HandlingType.CountOnly)
            {
                if (string.IsNullOrEmpty(rawMaterialDto.SubUnitName))
                {
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "اسم الوحدة الصغرى مطلوب للأصناف العددية."
                    };
                }
                rawMaterialDto.UnitWeightInKg = null;
            }

            if (!string.IsNullOrEmpty(rawMaterialDto.InnerUnitName) && rawMaterialDto.InnerUnitsPerSubUnit <= 1)
            {
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "لو عايز تضيف وحدة داخلية أصغر، لازم يكون عددها أكبر من 1."
                };
            }

            if (string.IsNullOrEmpty(rawMaterialDto.InnerUnitName) && rawMaterialDto.InnerUnitsPerSubUnit != 1)
            {
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "لازم تحدد اسم الوحدة الصغرى لما تحدد عدد أكبر من 1."
                };
            }

            rawMaterial.Name = rawMaterialDto.Name;
            rawMaterial.Description = rawMaterialDto.Description;
            rawMaterial.HandlingType = rawMaterialDto.HandlingType;
            rawMaterial.ContainerUnitName = rawMaterialDto.ContainerUnitName;
            rawMaterial.ContainerSize = rawMaterialDto.ContainerSize;
            rawMaterial.UnitType = rawMaterialDto.UnitType;
            rawMaterial.SubUnitName = rawMaterialDto.SubUnitName;
            rawMaterial.UnitWeightInKg = rawMaterialDto.UnitWeightInKg;
            rawMaterial.TotalContainerPrice = rawMaterialDto.TotalContainerPrice;
            rawMaterial.CategoryId = rawMaterialDto.CategoryId;
            rawMaterial.NormalStockLevel = rawMaterialDto.NormalStockLevel;
            rawMaterial.MeatFactor = rawMaterialDto.MeatFactor;
            rawMaterial.IsRestaurantSupply = rawMaterialDto.IsRestaurantSupply;
            rawMaterial.IsMeat = rawMaterialDto.IsMeat;
            rawMaterial.IsMultiplierLocked = rawMaterialDto.IsMultiplierLocked;

            rawMaterial.InnerUnitName = rawMaterialDto.InnerUnitName;
            rawMaterial.InnerUnitsPerSubUnit = rawMaterialDto.InnerUnitsPerSubUnit;
            rawMaterial.InnerUnitWeightInKg = rawMaterialDto.InnerUnitWeightInKg;

            if (rawMaterial.UnitWeightInKg.HasValue && rawMaterial.UnitWeightInKg.Value > 0)
            {
                var totalWeightInKg = rawMaterial.ContainerSize * rawMaterial.UnitWeightInKg.Value;
                rawMaterial.PricePerKg = totalWeightInKg > 0 ? rawMaterial.TotalContainerPrice / totalWeightInKg : 0;
            }
            else
            {
                rawMaterial.PricePerKg = null;
            }

            if (rawMaterial.ContainerSize > 0)
            {
                rawMaterial.PricePerPiece = rawMaterial.TotalContainerPrice / rawMaterial.ContainerSize;
            }
            else
            {
                rawMaterial.PricePerPiece = null;
            }

            var (_, _, pricePerInnerUnitCalc, _) = CalculatePricePerUnitWithDetails(rawMaterial);
            rawMaterial.PricePerInnerUnit = pricePerInnerUnitCalc;

            _unitOfWork.RawMaterialRepository.Update(rawMaterial);
            await _unitOfWork.CompleteAsync();

            var result = rawMaterial.Adapt<RawMaterialDetailsDto>();
            result.CategoryName = category.Name;
            result.IsWeightBased = rawMaterial.IsWeightBased;
            result.TotalWeightPerContainer = rawMaterial.TotalWeightPerContainer;

            var (pricePerUnit, pricePerKg, pricePerInnerUnit, displayUnit) = CalculatePricePerUnitWithDetails(rawMaterial);
            result.PricePerUnit = pricePerUnit;
            result.PricePerSubUnit = pricePerUnit;
            result.PricePerKg = pricePerKg;
            result.PricePerInnerUnit = pricePerInnerUnit;
            result.PriceDisplayUnit = displayUnit;

            result.InnerUnitName = rawMaterial.InnerUnitName;
            result.InnerUnitsPerSubUnit = rawMaterial.InnerUnitsPerSubUnit;
            result.InnerUnitWeightInKg = rawMaterial.InnerUnitWeightInKg;

            result.PricePerPiece = rawMaterial.PricePerPiece;

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم تحديث المادة الخام بنجاح",
                Data = result
            };
        }
        public async Task<Response> DeleteAsync(int id)
        {
            var rawMaterial = await _unitOfWork.RawMaterialRepository.GetAsync(id);
            if (rawMaterial == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المادة الخام مش موجودة"
                };
            _unitOfWork.RawMaterialRepository.Delete(rawMaterial);
            
            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "لا يمكن حذف المنتج"
                };
            }

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم حذف المادة الخام بنجاح"
            };
        }

        public async Task<Response> GetAllAsync(RawMaterialSpecParams specParams)
        {
            var spec = new RawMaterialSpecifications(specParams, isCount: false);
            var data = await _unitOfWork.RawMaterialRepository.GetAllWithSpecAsync(spec);

            var countSpec = new RawMaterialSpecifications(specParams, isCount: true);
            var count = await _unitOfWork.RawMaterialRepository.GetCountWithSpecAsync(countSpec);

            var result = new List<RawMaterialDetailsDto>();
            foreach (var item in data)
            {
                var dto = item.Adapt<RawMaterialDetailsDto>();

                var (pricePerUnit, pricePerKg, pricePerInnerUnit, displayUnit) = CalculatePricePerUnitWithDetails(item);
                dto.PricePerUnit = pricePerUnit;
                dto.PricePerSubUnit = pricePerUnit;
                dto.PricePerKg = pricePerKg;
                dto.PricePerInnerUnit = pricePerInnerUnit;
                dto.PriceDisplayUnit = displayUnit;

                dto.InnerUnitName = item.InnerUnitName;
                dto.InnerUnitsPerSubUnit = item.InnerUnitsPerSubUnit;
                dto.InnerUnitWeightInKg = item.InnerUnitWeightInKg;

                result.Add(dto);
            }

            var pagination = new Pagination(specParams.PageSize, specParams.PageIndex, count, result);

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استرجاع بيانات المواد الخام بنجاح",
                Data = pagination
            };
        }

        public async Task<Response> GetByIdAsync(int id)
        {
            var rawMaterial = await _unitOfWork.RawMaterialRepository.GetAsync(id);
            if (rawMaterial == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المادة الخام مش موجودة"
                };

            var result = rawMaterial.Adapt<RawMaterialDetailsDto>();

            var (pricePerUnit, pricePerKg, pricePerInnerUnit, displayUnit) = CalculatePricePerUnitWithDetails(rawMaterial);
            result.PricePerUnit = pricePerUnit;
            result.PricePerSubUnit = pricePerUnit;
            result.PricePerKg = pricePerKg;
            result.PricePerInnerUnit = pricePerInnerUnit;
            result.PriceDisplayUnit = displayUnit;

            result.InnerUnitName = rawMaterial.InnerUnitName;
            result.InnerUnitsPerSubUnit = rawMaterial.InnerUnitsPerSubUnit;
            result.InnerUnitWeightInKg = rawMaterial.InnerUnitWeightInKg;

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استرجاع بيانات المادة الخام بنجاح",
                Data = result
            };
        }

        private decimal CalculatePricePerUnit(RawMaterial rawMaterial)
        {
            if (rawMaterial.ContainerSize <= 0) return 0;

            if (rawMaterial.HandlingType == HandlingType.CountAndWeight &&
                rawMaterial.UnitWeightInKg.HasValue &&
                rawMaterial.UnitWeightInKg.Value > 0)
            {
                var totalWeight = rawMaterial.ContainerSize * rawMaterial.UnitWeightInKg.Value;
                return totalWeight > 0 ? rawMaterial.TotalContainerPrice / totalWeight : 0;
            }

            return rawMaterial.TotalContainerPrice / rawMaterial.ContainerSize;
        }

        private (decimal PricePerUnit, decimal? PricePerKg, decimal? PricePerInnerUnit, string DisplayUnit) CalculatePricePerUnitWithDetails(RawMaterial rawMaterial)
        {
            if (rawMaterial.ContainerSize <= 0)
                return (0, null, null, rawMaterial.UnitType.ToString());

            decimal pricePerSubUnit;
            decimal? pricePerKg = null;
            string displayUnit;

            if (rawMaterial.HandlingType == HandlingType.WeightOnly &&
                rawMaterial.UnitWeightInKg.HasValue &&
                rawMaterial.UnitWeightInKg.Value > 0)
            {
                var totalWeightInKg = rawMaterial.ContainerSize * rawMaterial.UnitWeightInKg.Value;
                pricePerKg = rawMaterial.TotalContainerPrice / totalWeightInKg;

                if (rawMaterial.UnitType == UnitType.g)
                {
                    pricePerSubUnit = pricePerKg.Value / 1000;
                    displayUnit = "g";
                }
                else
                {
                    pricePerSubUnit = pricePerKg.Value;
                    displayUnit = "kg";
                }
            }
            else if (rawMaterial.HandlingType == HandlingType.CountAndWeight &&
                     rawMaterial.UnitWeightInKg.HasValue &&
                     rawMaterial.UnitWeightInKg.Value > 0)
            {
                var totalWeight = rawMaterial.ContainerSize * rawMaterial.UnitWeightInKg.Value;
                pricePerKg = totalWeight > 0 ? rawMaterial.TotalContainerPrice / totalWeight : 0;

                if (rawMaterial.UnitType == UnitType.kg || rawMaterial.UnitType == UnitType.g)
                {
                    if (rawMaterial.UnitType == UnitType.g)
                    {
                        pricePerSubUnit = pricePerKg.Value / 1000;
                        displayUnit = "g";
                    }
                    else
                    {
                        pricePerSubUnit = pricePerKg.Value;
                        displayUnit = "kg";
                    }
                }
                else
                {
                    pricePerSubUnit = pricePerKg.Value * rawMaterial.UnitWeightInKg.Value;
                    displayUnit = rawMaterial.UnitType.ToString();
                }
            }
            else
            {
                pricePerSubUnit = rawMaterial.TotalContainerPrice / rawMaterial.ContainerSize;
                displayUnit = rawMaterial.UnitType.ToString();
            }

            decimal? pricePerInnerUnit = null;
            if (rawMaterial.InnerUnitsPerSubUnit > 0)
            {
                pricePerInnerUnit = pricePerSubUnit / rawMaterial.InnerUnitsPerSubUnit;
            }

            return (pricePerSubUnit, pricePerKg, pricePerInnerUnit, displayUnit);
        }

    }
}
