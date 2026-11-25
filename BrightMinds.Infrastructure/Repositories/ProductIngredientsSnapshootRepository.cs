using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;

namespace BrightMinds.Infrastructure.Repositories
{
    public class ProductIngredientsSnapshootRepository : GenericRepository<ProductIngredientSnapshoot>, IProductIngredientsSnapshootRepository
    {
        public ProductIngredientsSnapshootRepository(AppDbContext context) : base(context)
        {
        }
    }
}
