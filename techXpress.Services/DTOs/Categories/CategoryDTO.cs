using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace techXpress.Services.DTOs.CategoryDTOs
{
    public class CategoryDTO
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
    }

    public static class CategoryDTOMapping
    {
        public static Category ToCategory(this CategoryDTO categoryDTO)
        {
            return new Category
            {
                CategoryId = categoryDTO.CategoryId,
                Name = categoryDTO.Name
            };
        }
        public static CategoryDTO ToDTO(this Category category)
        {
            return new CategoryDTO
            {
                CategoryId = category.CategoryId,
                Name = category.Name
            };
        }

    }
}
