using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;
using BrightMinds.Services.IServices;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;
using BrightMinds.Services.IServices;
using Mapster;
using System.Net;
using BrightMinds.Services.Dtos.RecipeCategoryDto;

namespace BrightMinds.Services.Services
{

        public class RecipeCategoryService : IRecipeCategoryService
        {
            private readonly IUnitOfWork _unitOfWork;

            public RecipeCategoryService(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<Response> GetAllAsync()
            {
                var spec = new Core.Specifications.BaseSpecification<RecipeCategory>(x => !x.IsDeleted);
                spec.AddOrderBy(x => x.Name);

                var data = await _unitOfWork.RecipeCategoryRepository.GetAllWithSpecAsync(spec);
                var result = data.Adapt<List<RecipeCategoryDto>>();

                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "تم استرجاع التصنيفات بنجاح",
                    Data = result
                };
            }

            public async Task<Response> CreateAsync(string name)
            {
                if (string.IsNullOrWhiteSpace(name))
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "اسم التصنيف مطلوب"
                    };

                var existing = await _unitOfWork.RecipeCategoryRepository
                    .GetWithSpecAsync(new Core.Specifications.BaseSpecification<RecipeCategory>(x => x.Name == name && !x.IsDeleted));

                if (existing != null)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "هذا التصنيف موجود بالفعل"
                    };

                var category = new RecipeCategory { Name = name };
                await _unitOfWork.RecipeCategoryRepository.Add(category);
                await _unitOfWork.CompleteAsync();

                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.Created,
                    Message = "تم إضافة التصنيف بنجاح",
                    Data = category.Adapt<RecipeCategoryDto>()
                };
            }

            public async Task<Response> DeleteAsync(int id)
            {
                var category = await _unitOfWork.RecipeCategoryRepository.GetAsync(id);
                if (category == null || category.IsDeleted)
                    return new Response()
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = "التصنيف غير موجود"
                    };

                category.IsDeleted = true;
                _unitOfWork.RecipeCategoryRepository.Update(category);
                await _unitOfWork.CompleteAsync();

                return new Response()
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "تم حذف التصنيف بنجاح"
                };
            }
        }
    }