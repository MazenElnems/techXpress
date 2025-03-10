using DataAccessLayer.Abstraction;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace DataAccessLayer.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _db;

        public CategoryRepository(AppDbContext db)
        {
            _db = db;
        }

        public void Add(Category category)
        {
            _db.Categories.Add(category);
        }

        public IEnumerable<Category> GetAll()
        {
            return _db.Categories;
        }
    }
}