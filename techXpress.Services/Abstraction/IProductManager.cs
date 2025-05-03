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
        IEnumerable<ProductDTO> GetProductsWhere(ProductQueryDTO productQuery);
        IEnumerable<ProductDTO> GetProductsByCategory(int categoryId);
        void Delete(ProductDTO productDTO);
    }
}
