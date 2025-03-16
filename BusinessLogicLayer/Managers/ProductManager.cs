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
        private readonly IProductRepository _productRepository;

        public ProductManager(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public ProductDTO? GetById(int id)
        {
            Product? product = _productRepository.GetById(id);

            if(product != null) 
                return product.ToDto();

            return null;
        }

        public void Create(ProductDTO productDTO)
        {
            Product product = productDTO.ToProduct();
            _productRepository.Create(product);
        }

        public IEnumerable<ProductDTO> GetAll() 
        {
            var productDTOs = _productRepository.GetAll().Select(p => p.ToDto());
            return productDTOs;
        }

        public IEnumerable<ProductDTO> GetProductsByFilter(string searchTerm, string searchBy,
            decimal? minPrice, decimal? maxPrice,List<int> selectedCategories, bool all = false)
        {
            IEnumerable<Product> products = _productRepository.GetWithFilter(p =>
                (string.IsNullOrEmpty(searchTerm) || (searchBy == "name" ? p.Name.ToLower().Contains(searchTerm.Trim().ToLower()) : p.Description.ToLower().Contains(searchTerm.Trim().ToLower()))) &&
                (!minPrice.HasValue || p.Price >= minPrice.Value) &&
                (!maxPrice.HasValue || p.Price <= maxPrice.Value) &&
                all || selectedCategories.Contains(p.CategoryId)
            );

            return products.Select(p => p.ToDto());

        }
    }
}
