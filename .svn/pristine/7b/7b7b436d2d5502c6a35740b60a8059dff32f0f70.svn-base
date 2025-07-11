using System.ComponentModel.DataAnnotations;

namespace Core.ViewModel.Login
{
    public class ConfirmResetPasswordModel
    {
        public string Id { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required!")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters!")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Password confirmation is required!")]
        [MinLength(6, ErrorMessage = "Password confirmation must be at least 6 characters!")]
        [Compare("Password", ErrorMessage = "Passwords do not match!")]
        public string ConfirmPassword { get; set; }
    }
}