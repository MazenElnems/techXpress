using DataAccessLayer.Entities;
using System.Collections.Generic;

namespace DataAccessLayer.Abstraction
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAll();
        void Add(Category category);
    }
}