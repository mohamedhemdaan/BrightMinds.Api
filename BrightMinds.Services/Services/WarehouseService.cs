using BrightMinds.Core.Enums;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;
using BrightMinds.Core.Specifications;
using BrightMinds.Infrastructure.DataAccess;
using BrightMinds.Services.Dtos.ProductDtos;
using BrightMinds.Services.Dtos.WarehouseDtos;
using BrightMinds.Services.Dtos.WithdrawHistoryDtos;
using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Library.Services.Dtos;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Security.Claims;

namespace BrightMinds.Services.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public WarehouseService(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
        }

        private decimal? Convert(decimal quantity, UnitType sourceUnit, UnitType targetUnit)
        {
            if (sourceUnit == targetUnit)
                return quantity;

            if (sourceUnit == UnitType.g && targetUnit == UnitType.kg)
                return quantity / 1000;

            if (sourceUnit == UnitType.kg && targetUnit == UnitType.g)
                return quantity * 1000;

            return null;   
        }

        private async Task<bool> WithdrawFromSubWarehouse(int rawMaterialId, decimal quantity, UnitType unitType)
        {
            var inventoryItem = await _unitOfWork.WarehouseSuppliesInventoryRepository.GetFromSubWarehouseAsync(rawMaterialId);
            if (inventoryItem == null) return false;

            var requiredQuantity = (decimal)Convert(quantity, unitType, inventoryItem.UnitType);
            if (requiredQuantity>inventoryItem.Quantity)
            {
                return false;
            }
            else if(requiredQuantity == inventoryItem.Quantity)
            {
                _unitOfWork.WarehouseSuppliesInventoryRepository.Delete(inventoryItem);
                return true;
            }
            inventoryItem.Quantity -= requiredQuantity;
            _unitOfWork.WarehouseSuppliesInventoryRepository.Update(inventoryItem);
            return true;
        }

        private void CalculateStockLevelStatus(WarehouseInventoryItemDetailsDto inventoryItem, decimal normalStockLevel)
        {
            if (normalStockLevel <= 0)
            {
                inventoryItem.StockStatus = StockStatus.Undefined;
                return;
            }

            var percentage = (inventoryItem.Quantity / normalStockLevel) * 100;

            if (percentage >= 200)
                inventoryItem.StockStatus = StockStatus.VeryHigh;
            else if (percentage > 100 && percentage < 200)
                inventoryItem.StockStatus = StockStatus.High;
            else if (percentage == 100)
                inventoryItem.StockStatus = StockStatus.Normal;
            else if (percentage >= 50 && percentage < 100)
                inventoryItem.StockStatus = StockStatus.Low;
            else if (percentage >= 10 && percentage < 50)
                inventoryItem.StockStatus = StockStatus.VeryLow;
            else
                inventoryItem.StockStatus = StockStatus.Undefined;
        }

        private List<WarehouseInventoryItemDetailsDto> MapToWarehouseInventoryItemDetailsDto(IEnumerable<WarehouseSuppliesInventory> inventoryItems)
        {
            var result = new List<WarehouseInventoryItemDetailsDto>();
            foreach (var inventoryItem in inventoryItems)
            {
                var inventoryItemDetails = inventoryItem.Adapt<WarehouseInventoryItemDetailsDto>();
                CalculateStockLevelStatus(inventoryItemDetails, inventoryItem.SupplyItem.RawMaterial.NormalStockLevel);
                result.Add(inventoryItemDetails);
            }
            return result;
        }

        private async Task<Response> RecordRecipeIngredientsInWithdrawHistory(Recipe recipe)
        {
            var subWarehouse = await _unitOfWork.WarehouseRepository.GetByTypeAsync(WarehouseType.SubWarehouse);
            if (subWarehouse == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المخزن الفرعي مش موجود"
                };

            if (recipe == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "الوصفة مش موجودة"
                };

            var claims = _contextAccessor.HttpContext?.User?.Claims;
            if (claims == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = "لازم تسجل دخول الاول"
                };

            var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var firstName = claims?.FirstOrDefault(c => c.Type == "FirstName")?.Value;
            var lastName = claims?.FirstOrDefault(c => c.Type == "LastName")?.Value;

            var withdrawHistory = new WithdrawHistory()
            {
                NumberOfItems = recipe.Ingrediants.Count,
                RecipientName = $"{firstName} {lastName}".Trim(),
                UserId = userId,
                WarehouseId = subWarehouse.Id
            };

            foreach(var ingredient in recipe.Ingrediants)
            {
                if(ingredient.IngredientType == IngredientType.RawMaterial)
                {
                    withdrawHistory.WithdrawItems.Add(new WithdrawItem()
                    {
                        Name = ingredient.RawMaterial.Name,
                        Quantity = ingredient.Quantity,
                        UnitType = ingredient.UnitType
                    });
                }
                else if (ingredient.IngredientType == IngredientType.SubRecipe)
                {
                    withdrawHistory.WithdrawItems.Add(new WithdrawItem()
                    {
                        Name = ingredient.SubRecipe.Name,
                        Quantity = 1,
                        UnitType = UnitType.portion
                    });

                }
                else
                {
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "نوع المكون غير مدعوم"
                    };
                }
            }

            await _unitOfWork.WithdrawHistoryRepository.Add(withdrawHistory);
            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        public async Task<Response> GetAllWarehouses()
        {
            var warehouses = await _unitOfWork.WarehouseRepository.GetAllAsync();
            var result = warehouses.Adapt<List<WarehouseDetailsDto>>();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استعادة جميع المخازن بنجاح",
                Data = result
            };
        }

        public async Task<Response> GetAllSuppliesInventoryAsync(WarehouseInventorySpecParams specParams, WarehouseType warehouseType)
        {
            var warehouse = await _unitOfWork.WarehouseRepository.GetByTypeAsync(warehouseType);
            if (warehouse == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المخزن غير موجود"
                };

            var spec = new WarehouseSuppliesInventorySpecifications(specParams, warehouse.Id, null, false);
            var inventory = await _unitOfWork.WarehouseSuppliesInventoryRepository.GetAllWithSpecAsync(spec);

            var countSpec = new WarehouseSuppliesInventorySpecifications(specParams, warehouse.Id, null, true);
            var count = await _unitOfWork.WarehouseSuppliesInventoryRepository.GetCountWithSpecAsync(countSpec);

            var result = new List<WarehouseInventoryItemDetailsDto>();
            foreach (var item in inventory)
            {
                var rawMaterial = item.SupplyItem?.RawMaterial;
                var dto = new WarehouseInventoryItemDetailsDto
                {
                    Id = item.Id,
                    InventoryItemName = item.SupplyItem?.Name ?? "غير معروف",
                    Quantity = item.Quantity,
                    ContainerQuantity = item.ContainerQuantity ?? 0,
                    ContainerUnitName = rawMaterial?.ContainerUnitName ?? string.Empty,
                    DisplayDetails = item.DisplayDetails ?? $"{item.ContainerQuantity} {rawMaterial?.ContainerUnitName}",
                    Price = item.Price,
                    CategoryId = rawMaterial?.CategoryId ?? 0,
                    CategoryName = rawMaterial?.Category?.Name ?? "غير مصنف",
                    UnitType = item.UnitType.ToString(),
                };

                CalculateStockLevelStatus(dto, rawMaterial?.NormalStockLevel ?? 0);
                result.Add(dto);
            }

            var pagination = new Pagination(specParams.PageSize, specParams.PageIndex, count, result);

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استرجاع بيانات المخزون بنجاح",
                Data = pagination
            };
        }

        public async Task<Response> GetAllPrductsInventoryAsync(WarehouseProductsSpecParams specParams)
        {
            var spec = new WarehouseProductsSpecifications(specParams, false);
            var Products = await _unitOfWork.WarehouseProductsInventoryRepository.GetAllWithSpecAsync(spec);

            var countSpec = new WarehouseProductsSpecifications(specParams, true);
            var count = await _unitOfWork.WarehouseProductsInventoryRepository.GetCountWithSpecAsync(countSpec);

            var result = Products.Adapt<List<WarehouseProductDetailsDto>>();
            var pagination = new Pagination(specParams.PageSize, specParams.PageIndex, count, result);

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استعادة بيانات المنتجات بنجاح",
                Data = pagination
            };
        }

        public async Task<Response> WithdrawFromMain(MainInventoryWithdrawaDto withdrawlDto)
        {
            var mainWarehouse = await _unitOfWork.WarehouseRepository.GetByTypeAsync(WarehouseType.MainWarehouse);
            var subWarehouse = await _unitOfWork.WarehouseRepository.GetByTypeAsync(WarehouseType.SubWarehouse);

            if (mainWarehouse == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المخزن الرئيسي غير موجود"
                };
            if (subWarehouse == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المخزن الفرعي غير موجود"
                };

            var withdrawItems = new List<WithdrawItem>();
            var subWarehouseItems = new List<WarehouseSuppliesInventory>();

            foreach (var request in withdrawlDto.WithdrawRequests)
            {
                var inventoryItem = await _unitOfWork.WarehouseSuppliesInventoryRepository
                    .GetWithSpecAsync(new WarehouseSuppliesInventorySpecifications(null, null, request.Id, false));

                if (inventoryItem == null)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = $"العنصر {request.Id} غير موجود في المخزن الرئيسي"
                    };

                if (inventoryItem.WarehouseId != mainWarehouse.Id)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = $"العنصر {request.Id} غير موجود في المخزن الرئيسي"
                    };

                var rawMaterial = await _unitOfWork.RawMaterialRepository.GetAsync(inventoryItem.SupplyItem.RawMaterialId);
                if (rawMaterial == null)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = "المادة الخام غير موجودة"
                    };

                if (inventoryItem.ContainerQuantity < request.ContainerQuantity)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = $"كمية {rawMaterial.Name} غير كافية (المتاح: {inventoryItem.ContainerQuantity} {rawMaterial.ContainerUnitName})"
                    };

                inventoryItem.ContainerQuantity -= request.ContainerQuantity;
                inventoryItem.Quantity = (inventoryItem.ContainerQuantity ?? 0)  * (rawMaterial.ContainerSize);

                inventoryItem.DisplayDetails = rawMaterial.IsWeightBased ?
                    $"{inventoryItem.ContainerQuantity:F2} {rawMaterial.ContainerUnitName} ({inventoryItem.Quantity:F3} {rawMaterial.UnitType})" :
                    $"{inventoryItem.ContainerQuantity:F0} {rawMaterial.ContainerUnitName} = {inventoryItem.Quantity:F0} {rawMaterial.UnitType}";

                if (inventoryItem.ContainerQuantity == 0)
                    _unitOfWork.WarehouseSuppliesInventoryRepository.Delete(inventoryItem);
                else
                    _unitOfWork.WarehouseSuppliesInventoryRepository.Update(inventoryItem);

                var subItem = new WarehouseSuppliesInventory
                {
                    WarehouseId = subWarehouse.Id,
                    SupplyItemId = inventoryItem.SupplyItemId,
                    ContainerQuantity = request.ContainerQuantity,
                    Quantity = request.ContainerQuantity  * (rawMaterial.ContainerSize),
                    Price = inventoryItem.Price,
                    UnitType = rawMaterial.UnitType,
                    DisplayDetails = rawMaterial.IsWeightBased ?
                        $"{request.ContainerQuantity} {rawMaterial.ContainerUnitName} ({request.ContainerQuantity * rawMaterial.ContainerSize} {rawMaterial.UnitType})" :
                        $"{request.ContainerQuantity} {rawMaterial.ContainerUnitName} = {request.ContainerQuantity * rawMaterial.ContainerSize} {rawMaterial.UnitType}"
                };
                subWarehouseItems.Add(subItem);

                withdrawItems.Add(new WithdrawItem
                {
                    Name = rawMaterial.Name,
                    ContainerQuantity = request.ContainerQuantity,
                    Quantity = request.ContainerQuantity * (rawMaterial.ContainerSize),
                    UnitType = rawMaterial.UnitType,
                    DisplayDetails = subItem.DisplayDetails
                });
            }

            await _unitOfWork.WarehouseSuppliesInventoryRepository.AddInventoryItems(subWarehouseItems);

            var claims = _contextAccessor.HttpContext?.User?.Claims;
            var firstName = claims?.FirstOrDefault(c => c.Type == "FirstName")?.Value;
            var lastName = claims?.FirstOrDefault(c => c.Type == "LastName")?.Value;

            var withdrawHistory = new WithdrawHistory
            {
                WarehouseId = mainWarehouse.Id,
                NumberOfItems = withdrawlDto.WithdrawRequests.Count,
                RecipientName = withdrawlDto.RecipientName,
                UserId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier),
                Date = DateTime.UtcNow,
                WithdrawItems = withdrawItems
            };
            await _unitOfWork.WithdrawHistoryRepository.Add(withdrawHistory);
            await _unitOfWork.CompleteAsync();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم السحب للمخزن الفرعي بنجاح"
            };
        }

        public async Task<Response> GetWithrawHistory(WithdrawHistorySpecParams specParams)
        {
            var spec = new WithdrawHistorySpecifications(specParams, false);
            var data = await _unitOfWork.WithdrawHistoryRepository.GetAllWithSpecAsync(spec);

            var countSpec = new WithdrawHistorySpecifications(specParams, true);
            var count = await _unitOfWork.WithdrawHistoryRepository.GetCountWithSpecAsync(spec);

            var result = data.Adapt<List<WithdrawHistoryDetailsDto>>();
            var pagination = new Pagination(specParams.PageSize, specParams.PageIndex, count, result);

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استرجاع ارشيف السحب بنجاح",
                Data = pagination
            };
        }

        public async Task<Response> SettleProductAsync(int productId, decimal settledWeightInKg)
        {
            var product = await _unitOfWork.ProductRepository
                .GetWithSpecAsync(new ProdcutIncludesSpecifications(productId));
            if (product == null)
                return new Response() { StatusCode = 404, Message = "المنتج غير موجود" };

            if (product.ProductionStatus != ProductionStatus.SettlementRequired)
                return new Response() { StatusCode = 400, Message = "هذا المنتج ليس في مرحلة التسوية" };

            if (settledWeightInKg <= 0)
                return new Response() { StatusCode = 400, Message = "الوزن بعد التسوية يجب أن يكون أكبر من 0" };

            var recipe = await _unitOfWork.RecipeRepository.GetAsync(product.RecipeId);
            if (recipe == null)
                return new Response() { StatusCode = 404, Message = "الوصفة غير موجودة" };

            var initialWeight = product.InitialTotalWeight ?? recipe.TotalSize;
            var loss = initialWeight - settledWeightInKg;
            if (loss < 0)
                return new Response() { StatusCode = 400, Message = "الوزن بعد التسوية لا يمكن أن يتجاوز الوزن الابتدائي" };

            var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return new Response() { StatusCode = 401, Message = "يجب تسجيل الدخول لإتمام التسوية" };

            product.SettledWeight = settledWeightInKg;
            product.SettlementLoss = loss;
            product.SettledByUserId = userId;
            product.SettlementDate = DateTime.UtcNow;
            product.ProductionStatus = ProductionStatus.Waiting;

            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.CompleteAsync();

            return new Response()
            {
                StatusCode = 200,
                Message = $"تم تسجيل التسوية بنجاح، الفقد: {loss} كجم",
                Data = new
                {
                    product.Id,
                    SettledWeight = settledWeightInKg,
                    Loss = loss,
                    SettledByUserId = userId,
                    SettlementDate = product.SettlementDate
                }
            };
        }

        public async Task<Response> Produce(int? recipeId, bool isParent = true)
        {
            if (recipeId == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "معرف الوصفة مطلوب"
                };

            var recipe = await _unitOfWork.RecipeRepository.GetWithSpecAsync(new RecipeSpecifications(null, recipeId, false));
            if (recipe == null || recipe.IsDeleted)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "الوصفة غير موجودة"
                };

            var subWarehouse = await _unitOfWork.WarehouseRepository.GetByTypeAsync(WarehouseType.SubWarehouse);
            if (subWarehouse == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المخزن الفرعي غير موجود"
                };

            decimal productTotalCost = 0;
            var withdrawItems = new List<WithdrawItem>();
            var ingredientSnapshots = new List<ProductIngredientSnapshoot>();

            foreach (var ingredient in recipe.Ingrediants)
            {
                if (ingredient.IngredientType == IngredientType.SubRecipe)
                {
                    var subRecipeResult = await Produce(ingredient.SubRecipeId, false);
                    if (subRecipeResult.StatusCode != (int)HttpStatusCode.OK)
                        return subRecipeResult;
                    continue;
                }

                var rawMaterial = await _unitOfWork.RawMaterialRepository.GetAsync((int)ingredient.RawMaterialId);
                if (rawMaterial == null)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = $"المادة الخام غير موجودة"
                    };

                var inventoryItem = await _unitOfWork.WarehouseSuppliesInventoryRepository
                    .GetFromSubWarehouseAsync((int)ingredient.RawMaterialId);

                if (inventoryItem == null)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = $"ال {rawMaterial.Name} غير موجود في المخزن الفرعي"
                    };

                decimal requiredQuantity = ingredient.Quantity;

                if (inventoryItem.Quantity < requiredQuantity)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = $"كمية {rawMaterial.Name} في المخزن الفرعي غير كافية"
                    };

                productTotalCost += (inventoryItem.Price * requiredQuantity);
            }

            foreach (var ingredient in recipe.Ingrediants)
            {
                if (ingredient.IngredientType == IngredientType.SubRecipe)
                    continue;

                var rawMaterial = await _unitOfWork.RawMaterialRepository.GetAsync((int)ingredient.RawMaterialId);
                if (rawMaterial == null) continue;

                var inventoryItem = await _unitOfWork.WarehouseSuppliesInventoryRepository
                    .GetFromSubWarehouseAsync((int)ingredient.RawMaterialId);
                if (inventoryItem == null) continue;

                decimal requiredQuantity = ingredient.Quantity;

                if (rawMaterial.HandlingType == HandlingType.WeightOnly ||
                    rawMaterial.HandlingType == HandlingType.CountOnly)
                {
                    inventoryItem.Quantity -= requiredQuantity;
                }
                else if (rawMaterial.HandlingType == HandlingType.CountAndWeight)
                {
                    inventoryItem.Quantity -= requiredQuantity;
                }

                if (rawMaterial.ContainerSize > 0)
                {
                    inventoryItem.ContainerQuantity = inventoryItem.Quantity / rawMaterial.ContainerSize;
                }

                if (rawMaterial.UnitWeightInKg.HasValue)
                {
                    var totalWeight = inventoryItem.Quantity * rawMaterial.UnitWeightInKg.Value;
                    inventoryItem.DisplayDetails = rawMaterial.IsWeightBased ?
                        $"{inventoryItem.ContainerQuantity:F2} {rawMaterial.ContainerUnitName} ({inventoryItem.Quantity:F3} {rawMaterial.UnitType})" :
                        $"{inventoryItem.ContainerQuantity:F0} {rawMaterial.ContainerUnitName} = {inventoryItem.Quantity:F0} {rawMaterial.SubUnitName ?? rawMaterial.UnitType.ToString()} ({totalWeight:F3} كجم)";
                }
                else
                {
                    inventoryItem.DisplayDetails = rawMaterial.IsWeightBased ?
                        $"{inventoryItem.ContainerQuantity:F2} {rawMaterial.ContainerUnitName} ({inventoryItem.Quantity:F3} {rawMaterial.UnitType})" :
                        $"{inventoryItem.ContainerQuantity:F0} {rawMaterial.ContainerUnitName} = {inventoryItem.Quantity:F0} {rawMaterial.SubUnitName ?? rawMaterial.UnitType.ToString()}";
                }

                if (inventoryItem.Quantity == 0)
                    _unitOfWork.WarehouseSuppliesInventoryRepository.Delete(inventoryItem);
                else
                    _unitOfWork.WarehouseSuppliesInventoryRepository.Update(inventoryItem);

                withdrawItems.Add(new WithdrawItem()
                {
                    Name = rawMaterial.Name,
                    Quantity = requiredQuantity,
                    UnitType = rawMaterial.UnitType,
                    ContainerQuantity = rawMaterial.ContainerSize > 0 ? requiredQuantity / rawMaterial.ContainerSize : 0,
                    DisplayDetails = inventoryItem.DisplayDetails
                });
            }

            var initialStatus = recipe.RequiresSettlement
                 ? ProductionStatus.SettlementRequired
                 : ProductionStatus.Waiting;

            var product = new Product()
            {
                ExpectedQuantity = recipe.ExpectedPortiansnumber,
                Name = recipe.Name,
                RecipeId = recipe.Id,
                IsFlexible = recipe.IsFlexible,
                CreationDate = DateTime.UtcNow,
                ProductionStatus = initialStatus,
                InitialTotalWeight = recipe.RequiresSettlement ? recipe.TotalSize : null,
                Cost = productTotalCost       

            };
            if (isParent)
            {
                var claims = _contextAccessor.HttpContext?.User?.Claims;
                if (claims == null)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.Unauthorized,
                        Message = "يجب تسجيل الدخول أولاً"
                    };

                var firstName = claims.FirstOrDefault(c => c.Type == "FirstName")?.Value;
                var lastName = claims.FirstOrDefault(c => c.Type == "LastName")?.Value;
                var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

                var withdrawHistory = new WithdrawHistory()
                {
                    NumberOfItems = withdrawItems.Count,
                    RecipientName = $"{firstName} {lastName}".Trim(),
                    UserId = userId,
                    WarehouseId = subWarehouse.Id,
                    Date = DateTime.UtcNow,
                    WithdrawItems = withdrawItems
                };
                if (recipe.RequiresSettlement)
                {
                    product.WithdrawHistoryId = withdrawHistory.Id;     
                }
                await _unitOfWork.WithdrawHistoryRepository.Add(withdrawHistory);
                await _unitOfWork.ProductRepository.Add(product);
                await _unitOfWork.CompleteAsync();

                var result = product.Adapt<ProductDetailsDto>();
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "تم تصنيع المنتج بنجاح",
                    Data = result
                };
            }
            else
            {
                await _unitOfWork.ProductRepository.Add(product);
                await _unitOfWork.CompleteAsync();
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "تم تصنيع الوصفة الفرعية بنجاح",
                    Data = product
                };
            }
        }
     
        public async Task<Response> UpdateActualProductQuantities(ProductQuantitiesDto productQuantitiesDto)
        {
            var product = await _unitOfWork.ProductRepository.GetWithSpecAsync(new ProdcutIncludesSpecifications(productQuantitiesDto.ProductId));
            if (product == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المنتج مش موجود"
                };

            var recipe = await _unitOfWork.RecipeRepository.GetAsync(product.RecipeId);
            if (recipe == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "الوصفة المرتبطة بالمنتج غير موجودة"
                };

            if (product.IsFlexible)
            {
                decimal totalCalculatedSize = 0;

                foreach (var item in productQuantitiesDto.ProductQuantities)
                {
                    var portion = await _unitOfWork.RecipePortionRepository.GetWithSpecAsync(
                        new RecipePortionSpecifications(item.PortionId));

                    if (portion == null)
                        return new Response()
                        {
                            StatusCode = (int)HttpStatusCode.NotFound,
                            Message = $"التقسيمة المرتبطة بالمعرف {item.PortionId} مش موجودة"
                        };

                    if (portion.RecipeId != product.RecipeId)
                        return new Response()
                        {
                            StatusCode = (int)HttpStatusCode.BadRequest,
                            Message = "البورشن المٌدخل مش بتاع المنتج"
                        };

                    var productPortion = new ProductPortions()
                    {
                        Product = product,
                        ProductionStatus = ProductionStatus.Completed,
                        Quantity = item.NumberOfPortions,
                        SizeInGrams = portion.SizeInGrams,
                        Name = portion.Name,
                        Price = (recipe.CostPerKilo / 1000) * portion.SizeInGrams,
                        CategoryId = portion.CategoryId      
                    };

                    product.ProductPortions.Add(productPortion);
                    totalCalculatedSize += (item.NumberOfPortions * portion.SizeInGrams);
                }

                if (totalCalculatedSize > productQuantitiesDto.ActualSizeInGrams)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "الحجم الكلي للبورشنات المدخلة اكبر من الحجم الفعلي للمنتج"
                    };

                product.ActualQuantity = productQuantitiesDto.ProductQuantities.Sum(x => x.NumberOfPortions);
            }
            else
            {
                var portionSize = recipe.PortianSizeInGrams ?? 0;
                var portionPrice = (recipe.CostPerKilo / 1000) * portionSize;

                var productPortion = new ProductPortions()
                {
                    Product = product,
                    Name = product.Name,
                    ProductionStatus = ProductionStatus.Completed,
                    Quantity = (int)productQuantitiesDto.ActualQuantity,
                    SizeInGrams = portionSize,
                    Price = product.Recipe.TotalCost,
                    CategoryId = recipe.PortionCategoryId       
                };

                product.ProductPortions.Add(productPortion);
                product.ActualQuantity = productQuantitiesDto.ActualQuantity;
            }

            product.ProductionStatus = ProductionStatus.Completed;
            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.CompleteAsync();

            var result = product.Adapt<ProductDetailsDto>();
            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم اضافة الكمية/الكميات الفعلية بنجاح",
                Data = result
            };
        }
        public async Task<Response> TransfereProductPortionToSalesWarehouse(int productPortionId, bool saveChanges = true, WithdrawHistory withdrawHistory = null)
        {
            var salesWarehouse = await _unitOfWork.WarehouseRepository.GetByTypeAsync(WarehouseType.SalesWarehouse);
            if (salesWarehouse == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "مخزن البيع مش موجود",
                };
            var operationalWarehouse = await _unitOfWork.WarehouseRepository.GetByTypeAsync(WarehouseType.OperationalWarehouse);
            if (operationalWarehouse == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "مخزن التشغيل مش موجود",
                };

            var productPortion = await _unitOfWork.ProductPortionRepository.GetWithSpecAsync(new IncludeProductSpecifications(productPortionId));
            if (productPortion == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = $"البرسيلة المرتبطة بالمعرف {productPortionId} مش موجودة",
                };

            if(productPortion.ProductionStatus == ProductionStatus.Sent)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = $"تم ارسال البرسيلة المرتبطة بالمعرف {productPortionId} بالفعل",
                };

            if(productPortion.Product == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المنتج المطلوب غير موجود",
                };

            string displayDetails = $"{productPortion.Quantity} قطعة من {productPortion.Product?.Name ?? productPortion.Name} (حجم {productPortion.SizeInGrams} جرام)";

            if (withdrawHistory != null)
            {
                withdrawHistory.NumberOfItems += productPortion.Quantity;
                withdrawHistory.WithdrawItems.Add(new WithdrawItem()
                {
                    Name = productPortion.Product.Name,
                    WithdrawHistory = withdrawHistory,
                    Quantity = productPortion.Quantity,
                    UnitType = UnitType.piece,
                    DisplayDetails = displayDetails    

                });
            }
            else
            {
                var claims = _contextAccessor.HttpContext?.User?.Claims;
                if (claims == null)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.Unauthorized,
                        Message = "لازم تسجل دخول الاول"
                    };

                var firstName = claims?.FirstOrDefault(c => c.Type == "FirstName")?.Value;
                var lastName = claims?.FirstOrDefault(c => c.Type == "LastName")?.Value;

                WithdrawHistory newWithdrawHistory = new WithdrawHistory()
                {
                    NumberOfItems = productPortion.Quantity,
                    RecipientName = $"{firstName} {lastName}".Trim(),
                    UserId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier),
                    WarehouseId = operationalWarehouse.Id
                };
                newWithdrawHistory.WithdrawItems.Add(new WithdrawItem()
                {
                    Name = productPortion.Name,
                    WithdrawHistory = newWithdrawHistory,
                    Quantity = productPortion.Quantity,
                    UnitType = UnitType.piece,
                    DisplayDetails = displayDetails    

                });
                await _unitOfWork.WithdrawHistoryRepository.Add(newWithdrawHistory);
            }
            
            await _unitOfWork.WarehouseProductsInventoryRepository.AddProduct(productPortion);
            productPortion.ProductionStatus = ProductionStatus.Sent;
            productPortion.TransferredDate = DateTime.Now;
            if(productPortion.Product.ProductPortions.All(x=>x.ProductionStatus == ProductionStatus.Sent))
                productPortion.Product.ProductionStatus = ProductionStatus.Sent;

            _unitOfWork.ProductPortionRepository.Update(productPortion);
            _unitOfWork.ProductRepository.Update(productPortion.Product);
            if (saveChanges)
                await _unitOfWork.CompleteAsync();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم ترحيل المنتج الي مخزن البيع بنجاح",
            };

        }

        public async Task<Response> TransfereProductToSalesWarehouse(int productId)
        {
            var product = await _unitOfWork.ProductRepository.GetWithSpecAsync(new ProdcutIncludesSpecifications(productId));
            if (product == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المنتج مش موجود"
                };

            if (product.ActualQuantity == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = !product.IsFlexible? "لازم تضيف كمية الانتاج الفعلية للمنتج الاول" : "لازم تضيف كمية واحجام البرسيلات الاول"
                };

            var saleswarehouse = await _unitOfWork.WarehouseRepository.GetByTypeAsync(WarehouseType.SalesWarehouse);
            if (saleswarehouse == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "مخزن البيع مش موجود"
                };

            var operationalwarehouse = await _unitOfWork.WarehouseRepository.GetByTypeAsync(WarehouseType.OperationalWarehouse);
            if (operationalwarehouse == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "مخزن التشغيل مش موجود"
                };

            if(product.ProductionStatus == ProductionStatus.Sent)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "تم ارسال المنتج بالفعل"
                };

            var claims = _contextAccessor.HttpContext?.User?.Claims;
            if (claims == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = "لازم تسجل دخول الاول"
                };

            var firstName = claims?.FirstOrDefault(c => c.Type == "FirstName")?.Value;
            var lastName = claims?.FirstOrDefault(c => c.Type == "LastName")?.Value;

            WithdrawHistory withdrawHistory = new WithdrawHistory()
            {
                NumberOfItems = 0,
                RecipientName = $"{firstName} {lastName}".Trim(),
                UserId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier),
                WarehouseId = operationalwarehouse.Id
            };
            foreach(var productPortion in product.ProductPortions)
            {
                var res = await TransfereProductPortionToSalesWarehouse(productPortion.Id, false,  withdrawHistory);
                if (res.StatusCode != (int)HttpStatusCode.OK)
                    return res;
            }
            await _unitOfWork.WithdrawHistoryRepository.Add(withdrawHistory);
            await _unitOfWork.CompleteAsync();

            var result = product.Adapt<ProductDetailsDto>();
            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم ترحيل المنتجات لمخزن البيع بنجاح",
                Data = result
            };
        }

        public async Task<Response> GetDailyProduction()
        {
            var DailyProductions = await _unitOfWork.ProductRepository.GetDailyProduction();

            var result = DailyProductions.Adapt<List<ProductDetailsDto>>();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استعادة انتاج اليوم بنجاح",
                Data = result
            };
        }

        public async Task<Response> GetAllProductions(ProductionSpecParams specParams)
        {
            var spec = new ProductSpecifications(specParams, false);
            var data = await _unitOfWork.ProductRepository.GetAllWithSpecAsync(spec);

            var countSpec = new ProductSpecifications(specParams, true);
            var count = await _unitOfWork.ProductRepository.GetCountWithSpecAsync(countSpec);

            var result = data.Adapt<List<ProductDetailsDto>>();
            var pagination = new Pagination(specParams.PageSize, specParams.PageIndex, count, result);

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استرجاع الانتاج بنجاح",
                Data = pagination
            };
        }

        public async Task<Response> WithdrawFromSales(SalesInventoryWithdrawDto withdrawDto)
        {
            var salesWarehouse = await _unitOfWork.WarehouseRepository.GetByTypeAsync(WarehouseType.SalesWarehouse);
            if (salesWarehouse == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "مخزن المبيعات غير موجود"
                };

            var withdrawRequest = await _unitOfWork.SalesWarehouseWithdrawRequestRepository
                .GetWithSpecAsync(new SalesWarehouseWithdrawrequestSpecifications(withdrawDto.WithdrawRequestId));
            if (withdrawRequest == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "طلب السحب غير موجود"
                };

            var recipient = await _userManager.FindByIdAsync(withdrawRequest.Shift.UserId);
            if (recipient == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "المستخدم غير موجود"
                };

            withdrawDto.Products = withdrawDto.Products.GroupBy(x => x.Id)
                .Select(x => new ProductWithdrawDto()
                {
                    Id = x.Key,
                    Quantity = x.Sum(q => q.Quantity)
                }).ToList();

            WithdrawHistory withdrawHistory = new WithdrawHistory()
            {
                WarehouseId = salesWarehouse.Id,
                RecipientName = $"{recipient.FirstName} {recipient.LastName}".Trim(),
                NumberOfItems = withdrawDto.Products.Count,
                UserId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier),
                ChefShiftId = withdrawRequest.ShiftId
            };

            foreach (var productWithdraw in withdrawDto.Products)
            {
                var product = await _unitOfWork.WarehouseProductsInventoryRepository
                    .GetWithSpecAsync(new IncludeProductDetailsSpecifications(productWithdraw.Id));
                if (product == null)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = $"المنتج غير موجود"
                    };

                if (productWithdraw.Quantity > product.Quantity)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = $"كمية {product.ProductPortion.Product.Name} في مخزن البيع غير كافية"
                    };

                product.Quantity -= productWithdraw.Quantity;

                withdrawHistory.WithdrawItems.Add(new WithdrawItem()
                {
                    Name = product.ProductPortion.Product.Name,
                    Quantity = productWithdraw.Quantity,
                    UnitType = UnitType.piece,
                    WithdrawHistory = withdrawHistory,
                    DisplayDetails = $"سحب {productWithdraw.Quantity} قطعة من {product.ProductPortion.Product.Name}"
                });

                if (product.Quantity == 0)
                {
                    var requestedItems = await _unitOfWork.RequestedSalesWarehouseInventoryItemRepository
                        .GetAllWithSpecAsync(new BaseSpecification<RequestedSalesWarehouseInventoryItem>(x => x.InventoryItemId == product.Id));

                    foreach (var item in requestedItems)
                    {
                        _unitOfWork.RequestedSalesWarehouseInventoryItemRepository.Delete(item);
                    }

                    _unitOfWork.WarehouseProductsInventoryRepository.Delete(product);
                }
                else
                {
                    _unitOfWork.WarehouseProductsInventoryRepository.Update(product);
                }
            }

            await _unitOfWork.WithdrawHistoryRepository.Add(withdrawHistory);
            await _unitOfWork.CompleteAsync();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم سحب المنتجات من مخزن البيع بنجاح"
            };

        }

    }
}
