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
            try
            {
                _db.Categories.Add(category);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while creating category." , ex);
            }
        }

        public IEnumerable<Category> GetAll()
        {
            return _db.Categories;
        }

        public void Update(Category category)
        {
            try
            {
                _db.Categories.Update(category);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while updating category with category ID {category.CategoryId}." , ex);
            }
        }

        public void Delete(Category category) 
        {
            try
            {
                _db.Categories.Remove(category);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while removing category with category ID {category.CategoryId}.", ex);
            }
        }

        public Category? GetById(int id)
        {
            Category? category = _db.Find<Category>(id);

            return category;
        }


    }
}