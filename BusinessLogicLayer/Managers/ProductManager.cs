using BusinessLogicLayer.Abstraction;
using BusinessLogicLayer.DTOs;
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

        public void Create(ProductDTO productDTO)
        {
            Product product = new Product
            {
                Name = productDTO.Name,
                Price = productDTO.Price,
                StockQuantity = productDTO.StockQuantity,
                Image = productDTO.Image,
                Description = productDTO.Description
            };

            _productRepository.Create(product);
        }

        public IEnumerable<ProductDTO> GetAll() 
        {
            var productDTOs = _productRepository.GetAll().Select(p => new ProductDTO
            {
                Name = p.Name,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                Image = p.Image,
                Description = p.Description
            });
            return productDTOs;
        }

        public IEnumerable<ProductDTO> GetProductsByFilter(string searchTerm, string searchBy, decimal? minPrice, decimal? maxPrice)
        {
            IEnumerable<Product> products = _productRepository.GetWithFilter(p =>
                (string.IsNullOrEmpty(searchTerm) || (searchBy == "name" ? p.Name.ToLower().Contains(searchTerm.Trim().ToLower()) : p.Description.ToLower().Contains(searchTerm.Trim().ToLower()))) &&
                (!minPrice.HasValue || p.Price >= minPrice.Value) &&
                (!maxPrice.HasValue || p.Price <= maxPrice.Value) 
            );

            return products.Select(p => new ProductDTO
            {
                Name = p.Name,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                Image = p.Image,
                Description = p.Description
            });

        }
    }
}
