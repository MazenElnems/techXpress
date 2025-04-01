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
        IEnumerable<ProductDTO> GetAll(params string[]? Includes);
        IEnumerable<ProductDTO> GetProductsWhere(string? searchTerm, string? searchBy,
                    decimal minPrice, decimal maxPrice, int? categoryId);
        IEnumerable<ProductDTO> GetProductsByCategory(int categoryId);
    }
}
