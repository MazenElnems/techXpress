using BusinessLogicLayer.DTOs.CategoryDTOs;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.VMs.Category;

namespace PresentationLayer.ActionRequests
{
    public class UpdateCategoryActionRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public static class UpdateCategoryActionRequestMapping
    {
        public static CategoryDTO ToDto(this UpdateCategoryActionRequest request)
        {
            return new CategoryDTO { Name = request.Name };
        }
        public static CategoryVM ToVM(this UpdateCategoryActionRequest request) 
        {
            return new CategoryVM { Name = request.Name };
        }

        public static UpdateCategoryActionRequest ToActionRequest(this CategoryDTO categoryDTO)
        {
            return new UpdateCategoryActionRequest 
            {
                Name = categoryDTO.Name,
                Id = categoryDTO.CategoryId
            };

        }
    }

}
