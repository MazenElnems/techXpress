using BusinessLogicLayer.DTOs.CategoryDTOs;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Managers
{
    public class CategoryManager
    {
        private readonly CategoryRepository _categoryRepository;
        public CategoryManager(CategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public IEnumerable<CategoryDTO> GetAll()
        {
            IEnumerable<CategoryDTO> categories = _categoryRepository.GetAll()
                .Select(c => c.ToDTO());
            
            return categories;
        }

        public CategoryDTO? GetById(int id)
        {
            var category = _categoryRepository.GetById(id);
            if (category == null)
                return null;

            return category.ToDTO();
        }

        public void Create(CategoryCreateDTO categoryCreateDTO)
        {
            if(categoryCreateDTO == null)
                throw new ArgumentNullException(nameof(categoryCreateDTO));

            Category category = categoryCreateDTO.ToCategory();
            _categoryRepository.Add(category);
        }

        public void Update(CategoryDTO categoryDTO)
        {
            if (categoryDTO == null)
                throw new ArgumentNullException(nameof(categoryDTO));

            _categoryRepository.Update(categoryDTO.ToCategory());
        }

        public void Delete(int id) 
        {
            Category? category = _categoryRepository.GetById(id);

            if (category is null)
                throw new ApplicationException($"Category ID {id} not found");

            _categoryRepository.Delete(category);
        }
    }
}
