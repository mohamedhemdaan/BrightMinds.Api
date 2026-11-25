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
   
        public class MiscellaneousWithdrawRepository : GenericRepository<MiscellaneousWithdraw>, IMiscellaneousWithdrawRepository
        {
            public MiscellaneousWithdrawRepository(AppDbContext context) : base(context)
            {
            }
        }
    
}
