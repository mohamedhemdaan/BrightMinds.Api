using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;

namespace BrightMinds.Infrastructure.Repositories
{
    public class RestaurantSupplyWithdrawRepository : GenericRepository<RestaurantSupplyWithdraw>, IRestaurantSupplyWithdrawRepository
    {
        public RestaurantSupplyWithdrawRepository(AppDbContext context) : base(context)
        {
        }
    }
}