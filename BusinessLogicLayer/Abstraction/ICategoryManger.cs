using BusinessLogicLayer.DTOs.CategoryDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Abstraction
{
    public interface ICategoryManager
    {
        IEnumerable<CategoryDTO> GetAll(params string[]? Includes);
        CategoryDTO? GetById(int id);
        void Create(CategoryCreateDTO categoryCreateDTO);
        void Update(CategoryDTO categoryDTO);
        void Delete(int id);
        CategoryDTO? GetByName(string name);
    }
}
