using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModel.Login
{
    public class LoginModel
    {
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Username or mail is required!")]
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required!")]
        public string Password { get; set; }
        public string? Next { get; set; }
    }
}
