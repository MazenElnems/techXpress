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

        public IEnumerable<CategoryDTO> GetAll(params string[]? Includes)
        {
            IEnumerable<CategoryDTO> categories = _unitOfWork.CategoryRepository.GetAll(Includes)
                .Select(c => c.ToDTO());
            
            return categories;
        }

        public CategoryDTO? GetById(int id)
        {
            var category = _unitOfWork.CategoryRepository.GetById(c => c.CategoryId == id);
            if (category == null)
                return null;

            return category.ToDTO();
        }

        public void Create(CategoryCreateDTO categoryCreateDTO)
        {
            if(categoryCreateDTO == null)
                throw new ArgumentNullException(nameof(categoryCreateDTO));

            Category category = categoryCreateDTO.ToCategory();
            _unitOfWork.CategoryRepository.Create(category);
            _unitOfWork.Save();
        }

        public void Update(CategoryDTO categoryDTO)
        {
            if (categoryDTO == null)
                throw new ArgumentNullException(nameof(categoryDTO));

            _unitOfWork.CategoryRepository.Update(categoryDTO.ToCategory());
            _unitOfWork.Save();
        }

        public CategoryDTO? GetByName(string name)
        {
            CategoryDTO? categoryDTO = _unitOfWork.CategoryRepository
                .GetAllWhere(c => c.Name == name)
                .FirstOrDefault()?.ToDTO();

            return categoryDTO;    
        }

        public void Delete(int id) 
        {
            Category? category = _unitOfWork.CategoryRepository.GetById(c => c.CategoryId == id);
            if(category != null)
            {
                _unitOfWork.CategoryRepository.Delete(category);
                _unitOfWork.Save();
            }
        }
    }
}
