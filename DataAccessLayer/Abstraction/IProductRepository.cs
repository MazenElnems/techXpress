using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstraction
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetProductsByCategory(int categoryId);
        IEnumerable<Product> GetProductsBySeller(int sellerId);
        Product? GetProductWithDetails(int productId);
        void Create(Product product);
        IEnumerable<Product> GetAll();
        IEnumerable<Product> GetWithFilter(Expression<Func<Product, bool>> filter);
    }
}
