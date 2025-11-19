using System.ComponentModel.DataAnnotations;

namespace utb.loterie.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Zadejte uživatelské jméno")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Zadejte heslo")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}