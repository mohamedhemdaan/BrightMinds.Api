using BrightMinds.Core.Models;
using System;

namespace BrightMinds.Core.Interfaces
{
    public interface IRawMaterialRepository : IGenericRepository<RawMaterial>
    {
        public Task<bool> Exists(int id);
    }
}
