using Blendd.Infrastructure.DataAccess;
using BrightMinds.Core.Interfaces;
using BrightMinds.Core.Models;
using System;

namespace BrightMinds.Infrastructure.Repositories
{
    public class RawMaterialRepository : GenericRepository<RawMaterial>, IRawMaterialRepository
    {
        private readonly AppDbContext _context;

        public RawMaterialRepository(AppDbContext context):base(context) { }

        public async Task<bool> Exists(int id)
        {
           var rawMaterial =  await _context.RawMaterials.FindAsync(id);
           return rawMaterial != null;
        }
    }
}
