using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrightMinds.Core.Models
{
   
        public class RecipeCategory : BaseEntity
        {
            public string Name { get; set; }
            public bool IsDeleted { get; set; } = false;
        }
    }
