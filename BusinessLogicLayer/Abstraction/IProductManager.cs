using BusinessLogicLayer.DTOs.Products;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Abstraction
{
    public interface IProductManager
    {
        void Create(ProductDTO product);
        IEnumerable<ProductDTO> GetAll();
        IEnumerable<ProductDTO> GetProductsByFilter(string searchTerm, string searchBy,
            decimal? minPrice, decimal? maxPrice, List<int> selectedCategories,bool all);
    }
}
