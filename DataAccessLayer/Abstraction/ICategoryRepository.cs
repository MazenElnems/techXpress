using DataAccessLayer.Entities;
using System.Collections.Generic;

namespace DataAccessLayer.Abstraction
{
    public interface ICategoryRepository
    {
        void Add(Category category);
        IEnumerable<Category> GetAll();
        void Update(Category category);
        void Delete(Category category);
        Category? GetById(int id);
    }
}