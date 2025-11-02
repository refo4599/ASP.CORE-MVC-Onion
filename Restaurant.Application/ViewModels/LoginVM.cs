using System.ComponentModel.DataAnnotations;

namespace Restaurant.Application.ViewModels
{
    public class LoginVM
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
