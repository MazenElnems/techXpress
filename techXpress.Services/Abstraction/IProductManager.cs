using techXpress.Services.DTOs.Products;
using techXpress.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace techXpress.Services.Abstraction
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
        void Delete(ProductDTO productDTO);
    }
}
