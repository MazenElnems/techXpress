using System.ComponentModel.DataAnnotations;

namespace techXpress.UI.ActionRequests
{
    public class UpdateUserActionRequest
    {
        public Guid Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [Length(11, 11, ErrorMessage = "Phone number must be exactly 11 digits.")]
        public string PhoneNumber { get; set; }

        public string Role { get; set; }

        // Password fields are optional
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }
    }
}