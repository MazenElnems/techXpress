using BusinessLogicLayer.DTOs.CategoryDTOs;
using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.ActionRequests
{
    public class CreateCategoryActionRequest
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }

    public static class CreateCategoryActionRequestMapping
    {
        public static CategoryCreateDTO ToDto(this CreateCategoryActionRequest request)
        {
            return new CategoryCreateDTO { Name = request.Name };
        }
    }
}
