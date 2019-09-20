using System.ComponentModel.DataAnnotations;

namespace WebApp.Identity.Models
{
    public class RegisterModel
    {
        [EmailAddress(ErrorMessage = "Digite um e-mail válido")]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage ="Digite uma senha válida")]
        public string Password { get; set; }
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
