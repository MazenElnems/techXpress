using BusinessLogicLayer.Abstraction;
using BusinessLogicLayer.DTOs.Products;
using DataAccessLayer.Abstraction;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Managers
{
    public class ProductManager : IProductManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ProductDTO? GetById(int id)
        {
            Product? product = _unitOfWork.ProductRepository.GetById(p => p.Id == id);

            if(product != null) 
                return product.ToDto();

            return null;
        }

        public void Create(ProductDTO productDTO)
        {
            Product product = productDTO.ToProduct();
            _unitOfWork.ProductRepository.Create(product);
            _unitOfWork.Save();
        }

        public IEnumerable<ProductDTO> GetAll(params string[]? Includes) 
        {
            var productDTOs = _unitOfWork.ProductRepository.GetAll(Includes).Select(p => p.ToDto());
            return productDTOs;
        }

        public void Update(ProductDTO productDTO)
        {
            _unitOfWork.ProductRepository.Update(productDTO.ToProduct());
            _unitOfWork.Save();
        }

        public IEnumerable<ProductDTO> GetProductsWhere(string? searchTerm, string? searchBy,
            decimal minPrice, decimal maxPrice, int? categoryId)
        {
            IEnumerable<ProductDTO> products = _unitOfWork.ProductRepository.GetAllWhere(p =>
                (string.IsNullOrEmpty(searchTerm) ||
                    (searchBy == "name" ? p.Name.ToLower().Contains(searchTerm.ToLower().Trim())
                                         : p.Description.ToLower().Contains(searchTerm.ToLower().Trim())))
                && (minPrice == default || p.Price >= minPrice)
                && (maxPrice == default || p.Price <= maxPrice)
                && (categoryId == null || p.CategoryId == categoryId)
            )
            .Select(p => p.ToDto());

            return products;
        }

        public IEnumerable<ProductDTO> GetProductsByCategory(int categoryId)
        {
            IEnumerable<Product> products = _unitOfWork.ProductRepository.GetProductsByCategory(categoryId);
            return products.Select(p => p.ToDto());
        }

        public void Delete(ProductDTO productDTO)
        {
            try
            {
                _unitOfWork.ProductRepository.Delete(productDTO.ToProduct());
                _unitOfWork.Save();
            }
            catch(Exception ex)
            {
                throw new Exception("Product deletion failed", ex);
            }
        }
    }
}
