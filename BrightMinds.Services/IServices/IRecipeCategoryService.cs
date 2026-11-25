using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrightMinds.Services.IServices
{
   
        public interface IRecipeCategoryService
        {
            Task<Response> GetAllAsync();
            Task<Response> CreateAsync(string name);
            Task<Response> DeleteAsync(int id);      
        }
    }