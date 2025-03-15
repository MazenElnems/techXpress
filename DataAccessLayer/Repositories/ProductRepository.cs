using DataAccessLayer.Abstraction;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _db;

        public ProductRepository(AppDbContext db)
        {
            _db = db;
        }

        public void Create(Product product)
        {
            product.SellerId = 1;       // Default sellerId 
            if(product is null)
                throw new ArgumentNullException(nameof(product) + " is null");
            try
            {
                _db.Products.Add(product);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while adding {nameof(product)} to the database ", ex);
            }
        }

        public void Delete (Product product)
        {
            if (product is null)
                throw new ArgumentNullException(nameof(product) + " is null");
            try
            {
                _db.Products.Remove(product);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while removing {nameof(product)} ", ex);
            }
        }

        public void Update(Product product)
        {
            if (product is null)
                throw new ArgumentNullException(nameof(product) + " is null");
            try
            {
                _db.Products.Update(product);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while updating {nameof(product)} ", ex);
            }
        }

        public IEnumerable<Product> GetAll()
        {
            return _db.Products.ToList();
        }

        public IEnumerable<Product> GetProductsByCategory(int categoryId)
        {
            if (categoryId <= 0)
                throw new ArgumentException("Invalid category ID.", nameof(categoryId));

            try
            {
                return _db.Products.Where(p => p.CategoryId == categoryId).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving products for category ID {categoryId}.", ex);
            }
        }

        public IEnumerable<Product> GetProductsBySeller(int sellerId)
        {
            if (sellerId <= 0)
                throw new ArgumentException("Invalid seller ID.", nameof(sellerId));

            try
            {
                return _db.Products.Where(p => p.SellerId == sellerId).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving products for seller ID {sellerId}.", ex);
            }
        }

        public Product? GetProductWithDetails(int productId)
        {
            if (productId <= 0)
                throw new ArgumentException("Invalid product ID.", nameof(productId));

            try
            {
                return _db.Products
                    .Include(p => p.Category)
                    .Include(p => p.Seller)
                    .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                    .FirstOrDefault(p => p.Id == productId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving product details for product ID {productId}.", ex);
            }
        }

        public IEnumerable<Product> GetWithFilter(Expression<Func<Product,bool>> filter)
        {
            return _db.Products.Where(filter).ToList();
        }
    }
}
