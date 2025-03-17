using BusinessLogicLayer.Abstraction;
using BusinessLogicLayer.DTOs.CategoryDTOs;
using DataAccessLayer.Abstraction;
using DataAccessLayer.Entities;

namespace BusinessLogicLayer.Managers
{
    public class CategoryManager : ICategoryManager
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryManager(ICategoryRepository categoryRepository)
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
            _categoryRepository.Delete(id);
        }
    }
}
