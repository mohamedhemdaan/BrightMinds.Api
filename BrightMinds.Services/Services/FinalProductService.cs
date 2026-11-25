using BrightMinds.Core.Enums;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;
using BrightMinds.Core.Specifications;
using BrightMinds.Services.Dtos.FinalProductDtos;
using BrightMinds.Services.Dtos.FinalProductIngredientDtos;
using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Library.Services.Dtos;
using System.Net;

namespace BrightMinds.Services.Services
{
    public class FinalProductService : IFinalProductService
    {
        private readonly IUnitOfWork _uintOfWork;

        public FinalProductService(IUnitOfWork uintOfWork)
        {
            _uintOfWork = uintOfWork;
        }

        public async Task<Response> CreateFinalProduct(FinalProductDto finalProductDto)
        {
            var validation = await ValidateFinalProductDto(finalProductDto);
            if (validation.StatusCode != (int)HttpStatusCode.OK)
                return validation;

            if (finalProductDto.CategoryId.HasValue)
            {
                var category = await _uintOfWork.FinalProductCategoryRepository
                    .GetAsync(finalProductDto.CategoryId.Value);
                if (category == null || category.IsDeleted)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "التصنيف غير موجود"
                    };
            }

            if (finalProductDto.SubCategoryId.HasValue)
            {
                if (!finalProductDto.CategoryId.HasValue)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "يجب اختيار التصنيف الرئيسي أولاً عند اختيار تصنيف فرعي"
                    };

                var subCategory = await _uintOfWork.FinalProductSubCategoryRepository
                    .GetAsync(finalProductDto.SubCategoryId.Value);
                if (subCategory == null || subCategory.IsDeleted)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "التصنيف الفرعي غير موجود"
                    };

                if (subCategory.CategoryId != finalProductDto.CategoryId.Value)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "التصنيف الفرعي لا ينتمي إلى التصنيف الرئيسي المختار"
                    };
            }

            var finalProduct = MapToFinalProduct(finalProductDto);
            await _uintOfWork.FinalProductRepository.Add(finalProduct);
            await _uintOfWork.CompleteAsync();

            var result = await MapToFinalProductDetails(finalProduct);
            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم إنشاء المنتج النهائي بنجاح",
                Data = result
            };
        }

        public async Task<Response> UpdateFinalProduct(FinalProductDto finalProductDto, int id)
        {
            var validation = await ValidateFinalProductDto(finalProductDto, id);
            if (validation.StatusCode != (int)HttpStatusCode.OK)
                return validation;

            if (finalProductDto.CategoryId.HasValue)
            {
                var category = await _uintOfWork.FinalProductCategoryRepository
                    .GetAsync(finalProductDto.CategoryId.Value);
                if (category == null || category.IsDeleted)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "التصنيف غير موجود"
                    };
            }

            if (finalProductDto.SubCategoryId.HasValue)
            {
                if (!finalProductDto.CategoryId.HasValue)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "يجب اختيار التصنيف الرئيسي أولاً عند اختيار تصنيف فرعي"
                    };

                var subCategory = await _uintOfWork.FinalProductSubCategoryRepository
                    .GetAsync(finalProductDto.SubCategoryId.Value);
                if (subCategory == null || subCategory.IsDeleted)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "التصنيف الفرعي غير موجود"
                    };

                if (subCategory.CategoryId != finalProductDto.CategoryId.Value)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "التصنيف الفرعي لا ينتمي إلى التصنيف الرئيسي المختار"
                    };
            }
            var existingProduct = await _uintOfWork.FinalProductRepository.GetWithSpecAsync(new FinalProductSpecifications(id));
            if (existingProduct == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المنتج النهائي غير موجود"
                };

            existingProduct.Name = finalProductDto.Name;
            existingProduct.Description = finalProductDto.Description;
            existingProduct.SellingPrice = finalProductDto.SellingPrice;
            existingProduct.CategoryId = finalProductDto.CategoryId;  

            existingProduct.Ingredients.Clear();
            foreach (var ingredientDto in finalProductDto.Ingredients)
            {
                existingProduct.Ingredients.Add(MapToFinalProductIngredient(ingredientDto));
            }

            _uintOfWork.FinalProductRepository.Update(existingProduct);
            await _uintOfWork.CompleteAsync();

            var result = await MapToFinalProductDetails(existingProduct);
            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم تحديث المنتج النهائي بنجاح",
                Data = result
            };
        }

        public async Task<Response> DeleteFinalProduct(int id)
        {
            var finalProduct = await _uintOfWork.FinalProductRepository.GetAsync(id);
            if (finalProduct == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المنتج النهائي غير موجود"
                };

            _uintOfWork.FinalProductRepository.Delete(finalProduct);
            await _uintOfWork.CompleteAsync();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم حذف المنتج النهائي بنجاح"
            };
        }

        public async Task<Response> GetFinalProductById(int id)
        {
            var finalProduct = await _uintOfWork.FinalProductRepository.GetWithSpecAsync(new FinalProductSpecifications(id));
            if (finalProduct == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المنتج النهائي غير موجود"
                };

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استعادة المنتج النهائي بنجاح",
                Data = await MapToFinalProductDetails(finalProduct)
            };
        }

        public async Task<Response> GetAllFinalProducts(FinalProductSpecParams specParams)
        {
            var spec = new FinalProductSpecifications(specParams, false);
            var data = await _uintOfWork.FinalProductRepository.GetAllWithSpecAsync(spec);

            var countSpec = new FinalProductSpecifications(specParams, true);
            var count = await _uintOfWork.FinalProductRepository.GetCountWithSpecAsync(countSpec);

            var resultData = new List<FinalProductDetailsDto>();
            foreach (var item in data)
            {
                resultData.Add(await MapToFinalProductDetails(item));
            }
            var pagination = new Pagination(specParams.PageSize, specParams.PageIndex, count, resultData);

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استعادة المنتجات النهائية بنجاح",
                Data = pagination
            };
        }

        private async Task<Response> ValidateFinalProductDto(FinalProductDto finalProductDto, int? id = null)
        {
            if (id != null)
            {
                var finalProduct = await _uintOfWork.FinalProductRepository.GetWithSpecAsync(new FinalProductSpecifications((int)id));
                if (finalProduct == null)
                {
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = "المنتج النهائي غير موجود"
                    };
                }
            }

            foreach (var ingredientDto in finalProductDto.Ingredients)
            {
                var result = await ValidateFinalProductIngredientDto(ingredientDto);
                if (result.StatusCode != (int)HttpStatusCode.OK)
                    return result;
            }

            return new Response() { StatusCode = (int)HttpStatusCode.OK };
        }

        private async Task<Response> ValidateFinalProductIngredientDto(FinalProductIngredientDto finalProductIngredientDto)
        {
            switch (finalProductIngredientDto.Type)
            {
                case FinalProductIngredientType.RawMaterial:
                    var rawMaterial = await _uintOfWork.RawMaterialRepository.GetAsync(finalProductIngredientDto.Id);
                    if (rawMaterial == null)
                        return new Response()
                        {
                            StatusCode = (int)HttpStatusCode.NotFound,
                            Message = $"المادة الخام {finalProductIngredientDto.Id} غير موجودة"
                        };
                    if (rawMaterial.UnitType == UnitType.kg && finalProductIngredientDto.UnitType == UnitType.g)
                    {
                        finalProductIngredientDto.UnitType = UnitType.kg;
                        finalProductIngredientDto.Quantity /= 1000;
                    }
                    else if (rawMaterial.UnitType == UnitType.g && finalProductIngredientDto.UnitType == UnitType.kg)
                    {
                        finalProductIngredientDto.UnitType = UnitType.g;
                        finalProductIngredientDto.Quantity *= 1000;
                    }
                    break;

                case FinalProductIngredientType.ProductPortion:
                    var productPortion = await _uintOfWork.RecipePortionRepository.GetAsync(finalProductIngredientDto.Id);
                    if (productPortion == null)
                        return new Response()
                        {
                            StatusCode = (int)HttpStatusCode.NotFound,
                            Message = "البورشن غير موجود"
                        };
                    if (finalProductIngredientDto.UnitType == UnitType.g || finalProductIngredientDto.UnitType == UnitType.kg)
                        return new Response()
                        {
                            StatusCode = (int)HttpStatusCode.BadRequest,
                            Message = $"وحدة القياس غير المرتبطة بال {productPortion.Name} غير صحيحة"
                        };
                    break;
                default:
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "خطأ في نوع المكون"
                    };
            }
            return new Response() { StatusCode = (int)HttpStatusCode.OK };
        }

        #region before categoy

        #endregion
        private FinalProduct MapToFinalProduct(FinalProductDto finalProductDto)
        {
            var finalProduct = new FinalProduct()
            {
                Name = finalProductDto.Name,
                Description = finalProductDto.Description,
                SellingPrice = finalProductDto.SellingPrice,
                CategoryId = finalProductDto.CategoryId ,
                        SubCategoryId = finalProductDto.SubCategoryId  

            };

            foreach (var ingredient in finalProductDto.Ingredients)
                finalProduct.Ingredients.Add(MapToFinalProductIngredient(ingredient));

            return finalProduct;
        }

        private FinalProductIngredient MapToFinalProductIngredient(FinalProductIngredientDto finalProductIngredientDto)
        {
            switch (finalProductIngredientDto.Type)
            {
                case FinalProductIngredientType.RawMaterial:
                    return new RawMaterialIngredient()
                    {
                        Quantity = finalProductIngredientDto.Quantity,
                        Type = finalProductIngredientDto.Type,
                        RawMaterialId = finalProductIngredientDto.Id,
                        UnitType = finalProductIngredientDto.UnitType
                    };

                case FinalProductIngredientType.ProductPortion:
                    return new PortionIngredient()
                    {
                        ProductPortionId = finalProductIngredientDto.Id,
                        Quantity = finalProductIngredientDto.Quantity,
                        Type = finalProductIngredientDto.Type,
                        UnitType = finalProductIngredientDto.UnitType
                    };

                default:
                    throw new ArgumentException("خطأ في نوع المكون");
            }
        }

        private async Task<decimal> CalculateIngredientPrice(FinalProductIngredient finalProductIngredient)
        {
            decimal pricePerUnit = default;

            switch (finalProductIngredient.Type)
            {
                case FinalProductIngredientType.RawMaterial:
                    var rawMaterial = await _uintOfWork.RawMaterialRepository.GetAsync(((RawMaterialIngredient)finalProductIngredient).RawMaterialId);

                    if (rawMaterial != null)
                    {
                        var (pricePerUnitCalc, _, _) = CalculatePricePerUnitWithDetails(rawMaterial);
                        pricePerUnit = pricePerUnitCalc;
                    }
                    else
                    {
                        pricePerUnit = default;
                    }

                    break;
                case FinalProductIngredientType.ProductPortion:
                    var productPortion = await _uintOfWork.RecipePortionRepository.GetWithSpecAsync(new RecipePortionSpecifications(((PortionIngredient)finalProductIngredient).ProductPortionId));
                    pricePerUnit = ((productPortion?.Recipe.CostPerKilo / 1000) * productPortion?.SizeInGrams) ?? default;
                    break;
                default:
                    throw new ArgumentException("خطأ في نوع المكون");
            }

            return pricePerUnit * finalProductIngredient.Quantity;
        }

        private async Task<FinalProductDetailsDto> MapToFinalProductDetails(FinalProduct finalProduct)
        {
            var taxPrecentage = await _uintOfWork.FinalProductTaxRepository.GetUnifiedTaxPrecentage();
            decimal priceWithTax = default;

            if (finalProduct.SellingPrice != null)
            {
                priceWithTax = (decimal)(finalProduct.SellingPrice + (finalProduct.SellingPrice * (taxPrecentage / 100)));
            }

            var finalProductDetails = new FinalProductDetailsDto()
            {
                Id = finalProduct.Id,
                Name = finalProduct.Name,
                Description = finalProduct.Description,
                SellingPrice = finalProduct.SellingPrice ?? default,
                TaxPercentage = taxPrecentage,
                PriceWithTax = priceWithTax,

                CategoryId = finalProduct.CategoryId,
                CategoryName = finalProduct.Category?.Name,
                SubCategoryId = finalProduct.SubCategoryId,
                SubCategoryName = finalProduct.SubCategory?.Name
            };

            foreach (var ingredient in finalProduct.Ingredients)
                finalProductDetails.Ingredients.Add(await MapToFinalProductIngredientDetails(ingredient));

            finalProductDetails.TotalCost = finalProductDetails.Ingredients.Sum(x => x.Price);
            finalProductDetails.TotalCostPrecentage = default;

            if (finalProduct.SellingPrice != null && finalProduct.SellingPrice > 0)
                finalProductDetails.TotalCostPrecentage = (decimal)((finalProductDetails.TotalCost / finalProduct.SellingPrice) * 100);

            return finalProductDetails;
        }

        private async Task<FinalProductIngredientDetailsDto> MapToFinalProductIngredientDetails(FinalProductIngredient finalProductIngredient)
        {
            var finalProductIngredientDetails = new FinalProductIngredientDetailsDto()
            {
                Id = finalProductIngredient.Id,
                Quantity = finalProductIngredient.Quantity,
                Type = finalProductIngredient.Type,
                UnitType = finalProductIngredient.UnitType,
                Price = await CalculateIngredientPrice(finalProductIngredient),
            };

            switch (finalProductIngredient.Type)
            {
                case FinalProductIngredientType.RawMaterial:
                    var rawMaterial = await _uintOfWork.RawMaterialRepository.GetAsync(((RawMaterialIngredient)finalProductIngredient).RawMaterialId);
                    finalProductIngredientDetails.Name = rawMaterial?.Name;
                    break;

                case FinalProductIngredientType.ProductPortion:
                    var productPortion = await _uintOfWork.RecipePortionRepository.GetAsync(((PortionIngredient)finalProductIngredient).ProductPortionId);
                    finalProductIngredientDetails.Name = productPortion?.Name;
                    break;

                default:
                    throw new ArgumentException("خطأ في نوع المكون");
            }

            return finalProductIngredientDetails;
        }

        private (decimal PricePerUnit, decimal? PricePerKg, string DisplayUnit) CalculatePricePerUnitWithDetails(RawMaterial rawMaterial)
        {
            if (rawMaterial.ContainerSize <= 0)
                return (0, null, rawMaterial.UnitType.ToString());

            if (rawMaterial.HandlingType == HandlingType.WeightOnly &&
                rawMaterial.UnitWeightInKg.HasValue &&
                rawMaterial.UnitWeightInKg.Value > 0)
            {
                var totalWeightInKg = rawMaterial.ContainerSize * rawMaterial.UnitWeightInKg.Value;
                var pricePerKg = rawMaterial.TotalContainerPrice / totalWeightInKg;

                if (rawMaterial.UnitType == UnitType.g)
                {
                    var pricePerGram = pricePerKg / 1000;
                    return (pricePerGram, pricePerKg, "g");
                }
                else
                {
                    return (pricePerKg, pricePerKg, "kg");
                }
            }

            if (rawMaterial.HandlingType == HandlingType.CountAndWeight &&
                rawMaterial.UnitWeightInKg.HasValue &&
                rawMaterial.UnitWeightInKg.Value > 0)
            {
                var totalWeight = rawMaterial.ContainerSize * rawMaterial.UnitWeightInKg.Value;
                var pricePerKg = totalWeight > 0 ? rawMaterial.TotalContainerPrice / totalWeight : 0;

                if (rawMaterial.UnitType == UnitType.kg || rawMaterial.UnitType == UnitType.g)
                {
                    if (rawMaterial.UnitType == UnitType.g)
                    {
                        var pricePerGram = pricePerKg / 1000;
                        return (pricePerGram, pricePerKg, "g");
                    }
                    return (pricePerKg, pricePerKg, "kg");
                }
                else
                {
                    var pricePerUnit = pricePerKg * rawMaterial.UnitWeightInKg.Value;
                    return (pricePerUnit, pricePerKg, rawMaterial.UnitType.ToString());
                }
            }

            var price = rawMaterial.TotalContainerPrice / rawMaterial.ContainerSize;
            return (price, null, rawMaterial.UnitType.ToString());
        }

    }

    }
