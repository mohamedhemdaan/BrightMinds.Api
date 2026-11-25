using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;

namespace BrightMinds.Infrastructure.Repositories
{
    public class RecipePortionRepository : GenericRepository<RecipePortion>, IRecipePortionRepository
    {
        public RecipePortionRepository(AppDbContext context) : base(context)
        {
        }
    }
}
