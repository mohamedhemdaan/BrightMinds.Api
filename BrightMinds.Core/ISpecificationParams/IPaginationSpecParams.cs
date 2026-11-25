using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrightMinds.Core.ISpecificationParams
{

    public interface IPaginationSpecParams
    {
        int PageIndex { get; set; }
        int PageSize { get; set; }
        string? OrderBy { get; set; }
    }
}
