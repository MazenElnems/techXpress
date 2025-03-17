using BusinessLogicLayer.DTOs.CategoryDTOs;

namespace PresentationLayer.ActionRequests
{
    public class UpdateCategoryActionRequest
    {
        public string Name { get; set; }
    }

    public static class UpdateCategoryActionRequestMapping
    {
        public static CategoryDTO ToDto(this UpdateCategoryActionRequest request)
        {
            return new CategoryDTO { Name = request.Name };
        }
    }
}
