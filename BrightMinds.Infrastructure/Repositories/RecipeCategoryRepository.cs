using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrightMinds.Infrastructure.Repositories
{
   
        public class RecipeCategoryRepository : GenericRepository<RecipeCategory>, IRecipeCategoryRepository
        {
            public RecipeCategoryRepository(AppDbContext context) : base(context)
            {
            }
        }
    }