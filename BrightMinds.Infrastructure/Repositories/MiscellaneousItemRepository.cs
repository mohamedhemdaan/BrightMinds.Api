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
   public class MiscellaneousItemRepository : GenericRepository<MiscellaneousItem>, IMiscellaneousItemRepository
        {
            public MiscellaneousItemRepository(AppDbContext context) : base(context)
            {
            }
        }
    }


