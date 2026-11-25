using BrightMinds.Core.ISpecificationParams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrightMinds.Services.SpecificationParams
{
    
        public class MiscellaneousSpecParams : IMiscellaneousSpecParams
        {
            private const int MaxPageSize = 50;
            private int _pageSize = 10;

            public int PageIndex { get; set; } = 1;

            public int PageSize
            {
                get => _pageSize;
                set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
            }

            public string? OrderBy { get; set; } = "new";   

            public string? SearchName { get; set; }
        }
    }
