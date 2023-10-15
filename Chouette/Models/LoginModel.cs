using System.ComponentModel.DataAnnotations;

namespace Chouette.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Kullanıcı adı gereklidir!")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Şifre gereklidir!")]
        public string Password { get; set; }

        public string? ReturnUrl { get; set; }

        public bool RememberMe { get; set; }

    }
}
