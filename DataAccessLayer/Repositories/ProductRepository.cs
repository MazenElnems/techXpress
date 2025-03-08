using DataAccessLayer.Abstraction;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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
            _db.Products.Add(product);
            _db.SaveChanges();
        }

        public IEnumerable<Product> GetAll()
        {
            return _db.Products.ToList();
        }
    }
}
