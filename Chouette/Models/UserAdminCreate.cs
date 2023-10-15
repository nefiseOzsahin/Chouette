using System.ComponentModel.DataAnnotations;

namespace Chouette.Models
{
    public class UserAdminCreate
    {
        [Required(ErrorMessage = "Kullanıcı adı gereklidir!")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Cinsiyet gereklidir!")]
        public string Gender { get; set; }
        [EmailAddress(ErrorMessage = "Lütfen email formatı giriniz!")]
        [Required(ErrorMessage = "Email gereklidir!")]
        public string Email { get; set; }
    }
}
