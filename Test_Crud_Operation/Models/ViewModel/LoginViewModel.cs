using System.ComponentModel.DataAnnotations;

namespace Test_Crud_Operation.Models.ViewModel
{
    public class LoginViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
