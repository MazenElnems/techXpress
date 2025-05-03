using techXpress.Services.Abstraction;
using techXpress.Services.DTOs.Products;
using techXpress.DataAccess.Abstraction;
using techXpress.DataAccess.Entities;
using techXpress.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace techXpress.Services.Managers
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

        public IEnumerable<ProductDTO> GetProductsWhere(ProductQueryDTO productQuery)
        {
            IEnumerable<ProductDTO> products = _unitOfWork.ProductRepository.GetAllWhere(p => (productQuery.CategoryId == null || p.CategoryId == productQuery.CategoryId) &&
                     (string.IsNullOrEmpty(productQuery.SearchTerm) || p.Name.Contains(productQuery.SearchTerm)))
                     .Select(p => p.ToDto());

            if (productQuery.SortBy == "PriceAsc")
            {
                products = products.OrderBy(p => p.Price);
            }
            else if (productQuery.SortBy == "PriceDesc")
            {
                products = products.OrderByDescending(p => p.Price);
            }
            else if (productQuery.SortBy == "Name")
            {
                products = products.OrderBy(p => p.Name);
            }

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
