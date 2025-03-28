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
        ProductDTO? GetById(int id);
        void Create(ProductDTO product);
        void Update(ProductDTO productDTO);
        IEnumerable<ProductDTO> GetAll();
        IEnumerable<ProductDTO> GetProductsByFilter(string searchTerm, string searchBy,
            decimal? minPrice, decimal? maxPrice, List<int> selectedCategories,bool all);
        IEnumerable<ProductDTO> GetProductsByCategory(int categoryId);
    }
}
