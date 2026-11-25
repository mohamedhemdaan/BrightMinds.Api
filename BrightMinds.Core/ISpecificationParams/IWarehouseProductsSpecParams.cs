using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrightMinds.Core.ISpecificationParams
{
    public interface IWarehouseProductsSpecParams
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? SearchName { get; set; }
        public string? OrderBy { get; set; }

        public int? CategoryId { get; set; }      

    }
}
