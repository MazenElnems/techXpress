using BusinessLogicLayer.Abstraction;
using BusinessLogicLayer.DTOs.CategoryDTOs;
using DataAccessLayer.Abstraction;
using DataAccessLayer.Entities;

namespace BusinessLogicLayer.Managers
{
    public class CategoryManager : ICategoryManager
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<CategoryDTO> GetAll()
        {
            IEnumerable<CategoryDTO> categories = _unitOfWork.CategoryRepository.GetAll()
                .Select(c => c.ToDTO());
            
            return categories;
        }

        public CategoryDTO? GetById(int id)
        {
            var category = _unitOfWork.CategoryRepository.GetById(id);
            if (category == null)
                return null;

            return category.ToDTO();
        }

        public void Create(CategoryCreateDTO categoryCreateDTO)
        {
            if(categoryCreateDTO == null)
                throw new ArgumentNullException(nameof(categoryCreateDTO));

            Category category = categoryCreateDTO.ToCategory();
            _unitOfWork.CategoryRepository.Add(category);
        }

        public void Update(CategoryDTO categoryDTO)
        {
            if (categoryDTO == null)
                throw new ArgumentNullException(nameof(categoryDTO));

            _unitOfWork.CategoryRepository.Update(categoryDTO.ToCategory());
        }

        public void Delete(int id) 
        {
            _unitOfWork.CategoryRepository.Delete(id);
        }
    }
}
