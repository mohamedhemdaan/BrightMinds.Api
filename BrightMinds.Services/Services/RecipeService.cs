using BrightMinds.Core.Enums;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;
using BrightMinds.Core.Specifications;
using BrightMinds.Services.Dtos.RecipeDtos;
using BrightMinds.Services.Dtos.RecipeIngrediantDtos;
using BrightMinds.Services.Dtos.RecipePortionDtos;
using BrightMinds.Services.IServices;
using BrightMinds.Services.SpecificationParams;
using Library.Services.Dtos;
using Mapster;
using System.Net;

namespace BrightMinds.Services.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public RecipeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private async Task<decimal> IncludeRecipePrices(RecipeDetailsDto recipe, HashSet<int> visted = null)
        {
            visted??=new HashSet<int>();
            if (!visted.Add(recipe.Id)) return 0;

            decimal totalCost = default;
            foreach (var ingredient in recipe.Ingrediants)
            {
                if (ingredient.SubRecipeId != null)
                {
                    var subRecipe = await _unitOfWork.RecipeRepository.GetWithSpecAsync(new RecipeSpecifications(null, ingredient.SubRecipeId, false));
                    if(subRecipe == null)
                    {
                        ingredient.Price = 0;
                        continue;
                    }

                    ingredient.Price = await IncludeRecipePrices(subRecipe.Adapt<RecipeDetailsDto>());
                    totalCost += ingredient.Price;
                }
                else
                {
                    var rawMaterial = await _unitOfWork.RawMaterialRepository.GetAsync((int)ingredient.RawMaterialId);
                    if (rawMaterial == null)
                    {
                        ingredient.Price = 0;
                        continue;
                    }

                    var (pricePerUnit, _, _) = CalculatePricePerUnitWithDetails(rawMaterial);
                    decimal rawMaterialPrice = pricePerUnit;

                    if (rawMaterial.UnitType == UnitType.kg && ingredient.UnitType == UnitType.g)
                        rawMaterialPrice /= 1000;
                    else if (rawMaterial.UnitType == UnitType.g && ingredient.UnitType == UnitType.kg)
                        rawMaterialPrice *= 1000;

                    ingredient.Price = rawMaterialPrice * (ingredient.Quantity);
                    totalCost += ingredient.Price;
                }
            }

            recipe.TotalCost = totalCost;

            return totalCost;
        }

        public async Task<Response> GetAllAsync(RecipeSpecParams specParams)
        {
            var spec = new RecipeSpecifications(specParams, null, isCount: false);
            var data = await _unitOfWork.RecipeRepository.GetAllWithSpecAsync(spec);

            var countSpec = new RecipeSpecifications(specParams, null, isCount: true);
            var count = await _unitOfWork.RecipeRepository.GetCountWithSpecAsync(countSpec);

            var result = data.Adapt<List<RecipeDetailsDto>>();
            foreach (var item in result)
            {
                await IncludeRecipePrices(item);
            }
            var pagination = new Pagination(specParams.PageSize, specParams.PageIndex, count, result);

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استعادة بيانات الوصفات بنجاح",
                Data = pagination
            };
        }

        private async Task<decimal> CalculateTotalSize(RecipeDto recipeDto)
        {
            decimal totalSize = 0;

            foreach (var ingredientDto in recipeDto.Ingrediants)
            {
                if (ingredientDto.IngredientType == IngredientType.RawMaterial)
                {
                    var rawMaterial = await _unitOfWork.RawMaterialRepository.GetAsync((int)ingredientDto.RawMaterialId);
                    if (rawMaterial == null) continue;

                    decimal quantityInKg = 0;

                    if (rawMaterial.UnitType == UnitType.kg || rawMaterial.UnitType == UnitType.g)
                    {
                        if (ingredientDto.UnitType == UnitType.kg)
                            quantityInKg = ingredientDto.Quantity;
                        else if (ingredientDto.UnitType == UnitType.g)
                            quantityInKg = ingredientDto.Quantity / 1000;
                    }
                    else
                    {
                        if (rawMaterial.UnitWeightInKg.HasValue && rawMaterial.UnitWeightInKg.Value > 0)
                        {
                            quantityInKg = ingredientDto.Quantity * rawMaterial.UnitWeightInKg.Value;
                        }
                        else
                        {
                            quantityInKg = 0;
                        }
                    }

                    totalSize += quantityInKg;
                }
                else if (ingredientDto.IngredientType == IngredientType.SubRecipe)
                {
                    var subRecipe = await _unitOfWork.RecipeRepository.GetAsync((int)ingredientDto.SubRecipeId);
                    if (subRecipe != null)
                    {
                        totalSize += subRecipe.TotalSize * ingredientDto.Quantity;
                    }
                }
            }

            return totalSize;
        }

        public async Task<Response> CreateAsync(RecipeDto recipeDto)
        {
            foreach (RecipeIngrediantDto ingrediant in recipeDto.Ingrediants)
            {
                if (ingrediant.IngredientType == IngredientType.RawMaterial)
                {
                    var rawMaterial = await _unitOfWork.RawMaterialRepository.GetAsync((int)ingrediant.RawMaterialId);
                    if (rawMaterial == null)
                        return new Response()
                        {
                            StatusCode = (int)HttpStatusCode.NotFound,
                            Message = $"مش موجودة {ingrediant.RawMaterialId} المادة الخام المرتبطة بالمعرفة"
                        };

                    if (!((rawMaterial.UnitType == UnitType.kg || rawMaterial.UnitType == UnitType.g) &&
                           (ingrediant.UnitType == UnitType.kg || ingrediant.UnitType == UnitType.g)))
                    {
                        if (rawMaterial.UnitType != ingrediant.UnitType)
                            return new Response()
                            {
                                StatusCode = (int)HttpStatusCode.BadRequest,
                                Message = $"وحدة القياس بتاعة ال {rawMaterial.Name} غلط"
                            };
                    }
                }
                else if (ingrediant.IngredientType == IngredientType.SubRecipe)
                {
                    var subRecipe = await _unitOfWork.RecipeRepository.GetAsync((int)ingrediant.SubRecipeId);
                    if (subRecipe == null || subRecipe.IsDeleted)
                        return new Response()
                        {
                            StatusCode = (int)HttpStatusCode.NotFound,
                            Message = $"الوصفة المرتبطة بالمعرف {ingrediant.SubRecipeId} مش موجودة"
                        };
                }
            }

            var recipe = recipeDto.Adapt<Recipe>();

            if (!recipeDto.IsFlexible && recipeDto.PortionCategoryId.HasValue)
            {
                var categoryExists = await _unitOfWork.PortionCategoryRepository
                    .GetAsync(recipeDto.PortionCategoryId.Value);
                if (categoryExists == null)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "تصنيف البورشن غير موجود"
                    };
                recipe.PortionCategoryId = recipeDto.PortionCategoryId;
            }

            var totalCost = await IncludeRecipePrices(recipe.Adapt<RecipeDetailsDto>());
            recipe.TotalCost = totalCost;

            var calculatedTotalSize = await CalculateTotalSize(recipeDto);

            var lossInKg = recipe.NormalLossSize / 1000;     
            recipe.TotalSize = calculatedTotalSize - lossInKg;      

            recipe.CostPerKilo = recipe.TotalSize > 0 ? recipe.TotalCost / (recipe.TotalSize / 1000) : 0;

            await _unitOfWork.RecipeRepository.Add(recipe);
            await _unitOfWork.CompleteAsync();

            var result = recipe.Adapt<RecipeDetailsDto>();
            await IncludeRecipePrices(result);

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.Created,
                Message = "تم انشاء الوصفة بنجاح",
                Data = result
            };
        }
        public async Task<Response> UpdateAsync(RecipeDto recipeDto, int id)
        {
            var recipe = await _unitOfWork.RecipeRepository.GetWithSpecAsync(new RecipeSpecifications(null, id, false));
            if (recipe == null || recipe.IsDeleted)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = $"مش موجودة {id} الوصفة المرتبطة بالمعرفة"
                };

            recipe.Name = recipeDto.Name;
            recipe.Category = recipeDto.Category;
            recipe.PortionCategoryId = recipeDto.PortionCategoryId;
            recipe.IsFlexible = recipeDto.IsFlexible;
            recipe.IsSubRecipe = recipeDto.IsSubRecipe;
            recipe.RequiresSettlement = recipeDto.RequiresSettlement;
            recipe.NormalLossSize = recipeDto.NormalLossSize;

            recipe.Ingrediants.Clear();
            foreach (RecipeIngrediantDto ingrediantDto in recipeDto.Ingrediants)
            {
                if (ingrediantDto.IngredientType == IngredientType.RawMaterial)
                {
                    var rawMaterial = await _unitOfWork.RawMaterialRepository.GetAsync((int)ingrediantDto.RawMaterialId);
                    if (rawMaterial == null)
                        return new Response()
                        {
                            StatusCode = (int)HttpStatusCode.NotFound,
                            Message = $"مش موجودة {ingrediantDto.RawMaterialId} المادة الخام المرتبطة بالمعرفة"
                        };

                    if (!((rawMaterial.UnitType == UnitType.kg || rawMaterial.UnitType == UnitType.g) &&
                           (ingrediantDto.UnitType == UnitType.kg || ingrediantDto.UnitType == UnitType.g)))
                    {
                        if (rawMaterial.UnitType != ingrediantDto.UnitType)
                            return new Response()
                            {
                                StatusCode = (int)HttpStatusCode.BadRequest,
                                Message = $"وحدة القياس بتاعة ال {rawMaterial.Name} غلط"
                            };
                    }
                }
                else if (ingrediantDto.IngredientType == IngredientType.SubRecipe)
                {
                    var subRecipe = await _unitOfWork.RecipeRepository.GetAsync((int)ingrediantDto.SubRecipeId);
                    if (subRecipe == null)
                        return new Response()
                        {
                            StatusCode = (int)HttpStatusCode.NotFound,
                            Message = $"مش موجودة {ingrediantDto.SubRecipeId} الوصفة الفرعية المرتبطة بالمعرفة"
                        };
                }

                var ingrediant = ingrediantDto.Adapt<RecipeIngrediant>();
                ingrediant.RecipeId = id;
                recipe.Ingrediants.Add(ingrediant);
            }

            var totalCost = await IncludeRecipePrices(recipe.Adapt<RecipeDetailsDto>());
            recipe.TotalCost = totalCost;

            var calculatedTotalSize = await CalculateTotalSize(recipeDto);
            var lossInKg = recipe.NormalLossSize / 1000;
            recipe.TotalSize = calculatedTotalSize - lossInKg;

            if (recipe.TotalSize < 0)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "معدل الفقد الطبيعي أكبر من الوزن الكلي للوصفة"
                };

            recipe.CostPerKilo = recipe.TotalSize > 0 ? recipe.TotalCost / (recipe.TotalSize / 1000) : 0;

            _unitOfWork.RecipeRepository.Update(recipe);
            await _unitOfWork.CompleteAsync();

            var result = recipe.Adapt<RecipeDetailsDto>();
            await IncludeRecipePrices(result);

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم تحديث الوصفة بنجاح",
                Data = result
            };
        }

        public async Task<Response> DeleteAsync(int id)
        {
            var recipe = await _unitOfWork.RecipeRepository.GetAsync(id);
            if (recipe == null || recipe.IsDeleted)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "الوصفة مش موجودة"
                };

            recipe.IsDeleted = true;
            _unitOfWork.RecipeRepository.Update(recipe);
            await _unitOfWork.CompleteAsync();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم حذف الوصفة بنجاح"
            };
        }

        public async Task<Response> GetByIdAsync(int id)
        {
            var recipe = await _unitOfWork.RecipeRepository.GetWithSpecAsync(new RecipeSpecifications(null, id, false));
            if (recipe == null || recipe.IsDeleted)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "الوصفة مش موجوده"
                };
            var result = recipe.Adapt<RecipeDetailsDto>();
            await IncludeRecipePrices(result);
            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استعادة الوصفة بنجاح",
                Data = result
            };
        }

        public async Task<Response> AddRecipePortions(RecipePortionDto recipePortions)
        {
            var recipe = await _unitOfWork.RecipeRepository.GetAsync(recipePortions.RecipeId);
            if (recipe == null || recipe.IsDeleted)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "الوصفة مش موجودة"
                };

            if (recipe.IsSubRecipe)
            {
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "مينفعش الوصفة الفرعية يكون ليها احجام مختلفة"
                };
            }

            if (!recipe.IsFlexible)
            {
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "مينفعش الوصفة الثابتة يكون ليها احجام مختلفة"
                };
            }

            foreach(var portion in recipePortions.Portions)
            {
                recipe.Portions.Add(new RecipePortion()
                {
                    Name = portion.Name,
                    SizeInGrams = portion.SizeInGrams,
                    Recipe = recipe,

                    CategoryId = portion.CategoryId    

                });
            }
            _unitOfWork.RecipeRepository.Update(recipe);
            await _unitOfWork.CompleteAsync();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم اضافة تقسيمات الوصفة بنجاح"
            };
        }

        public async Task<Response> GetAllRecipesPortions(RecipePortionsSpecParams specParams)
        {
            var spec = new RecipePortionSpecifications(specParams, false);
            var data = await _unitOfWork.RecipePortionRepository.GetAllWithSpecAsync(spec);

            var countSpec = new RecipePortionSpecifications(specParams, true);
            var count = await _unitOfWork.RecipePortionRepository.GetCountWithSpecAsync(countSpec);

            var result = data.GroupBy(x => x.RecipeId)
                             .Select(x =>
                             {
                                 var recipeName = x.First().Recipe.Name;

                                 return new RecipePortionsDetailsDto()
                                 {
                                     RecipeId = x.Key,
                                     RecipeName = recipeName,
                                     Portions = x.ToList().Adapt<List<PortionDetailsDto>>()
                                 };
                             });

            var pagination = new Pagination(specParams.PageSize, specParams.PageIndex, count, result);

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استرجاع تقسيمات الوصفات بنجاح",
                Data = pagination
            };
        }

        public async Task<Response> GetRecipePortionById(int id)
        {
            var recipePortion = await _unitOfWork.RecipePortionRepository.GetWithSpecAsync(new RecipePortionIncludesSpecifications(id));
            if (recipePortion == null || recipePortion.Recipe == null || recipePortion.Recipe.IsDeleted)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "التقسيمة مش موجودة"
                };

            var result = new RecipePortionsDetailsDto()
            {
                RecipeId = recipePortion.RecipeId,
                RecipeName = recipePortion.Recipe.Name
            };
            result.Portions.Add(recipePortion.Adapt<PortionDetailsDto>());

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم استرجاع التقسيمة بنجاح",
                Data = result
            };
        }

        public async Task<Response> UpdateRecipePortion(RecipePortionDto recipePortionDto)
        {
            var recipe = await _unitOfWork.RecipeRepository.GetWithSpecAsync(new RecipeSpecifications(null, recipePortionDto.RecipeId, false));
            if (recipe == null || recipe.IsDeleted)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "الوصفة مش موجودة"
                };

            if (recipe.IsSubRecipe)
            {
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "مينفعش الوصفة الفرعية يكون ليها احجام مختلفة"
                };
            }

            if (!recipe.IsFlexible)
            {
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "مينفعش الوصفة الثابتة يكون ليها احجام مختلفة"
                };
            }

            _unitOfWork.RecipePortionRepository.DeleteRange(recipe.Portions);
            foreach(var portion in recipePortionDto.Portions)
            {
                recipe.Portions.Add(new RecipePortion()
                {
                    Name = portion.Name,
                    SizeInGrams = portion.SizeInGrams,
                    Recipe = recipe
                });
            }
            _unitOfWork.RecipeRepository.Update(recipe);
            await _unitOfWork.CompleteAsync();

            var result = new RecipePortionsDetailsDto()
            {
                RecipeId = recipe.Id,
                RecipeName = recipe.Name
            };
            result.Portions = recipe.Portions.Adapt<List<PortionDetailsDto>>();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم تعديل تقسيمة الوصفة بنجاح",
                Data = result
            };
        }
    
        public async Task<Response> DeleteRecipePortion(int id)
        {
            var recipePortion = await _unitOfWork.RecipePortionRepository.GetAsync(id);
            if (recipePortion == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "التقسيمة مش موجودة"
                };

            _unitOfWork.RecipePortionRepository.Delete(recipePortion);

            await _unitOfWork.CompleteAsync();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم حذف التقسيمة بنجاح"
            };
        }
    
        public async Task<Response> DeleteAllRecipePortions(int recipeId)
        {
            var recipe = await _unitOfWork.RecipeRepository.GetWithSpecAsync(new RecipeSpecifications(null, recipeId, false));
            if (recipe == null)
                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "الوصفة مش موجودة"
                };

            _unitOfWork.RecipePortionRepository.DeleteRange(recipe.Portions);
            _unitOfWork.RecipeRepository.Update(recipe);
            await _unitOfWork.CompleteAsync();

            return new Response()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "تم حذف تقسيمات الوصفة بنجاح"
            };
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
