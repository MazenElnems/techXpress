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
                _db.SaveChanges();
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
                Category? categoryToUpdate = _db.Categories.FirstOrDefault(c => c.CategoryId == category.CategoryId);
                if(categoryToUpdate != null) 
                {                
                    categoryToUpdate.Name = category.Name;
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while updating category with category ID {category.CategoryId}." , ex);
            }
        }

        public void Delete(int id) 
        {
            try
            {
                Category? categoryToDelete = _db.Categories.FirstOrDefault(c => c.CategoryId == id);
                if(categoryToDelete != null)
                {
                    _db.Categories.Remove(categoryToDelete);
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while removing category with category ID {id}.", ex);
            }
        }

        public Category? GetById(int id)
        {
            Category? category = _db.Find<Category>(id);

            return category;
        }


    }
}