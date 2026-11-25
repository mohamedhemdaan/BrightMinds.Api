using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;

namespace BrightMinds.Infrastructure.Repositories
{
    public class RecipeRepository : GenericRepository<Recipe>, IRecipeRepository
    {
        public RecipeRepository(AppDbContext context) : base(context)
        {
        }
    }
}
