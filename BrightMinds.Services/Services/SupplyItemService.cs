using BrightMinds.Core.Interfaces;
using BrightMinds.Services.IServices;

namespace BrightMinds.Services.Services
{
    public class SupplyItemService : ISupplyItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SupplyItemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
