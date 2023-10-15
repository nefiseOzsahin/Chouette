using System.ComponentModel.DataAnnotations;

namespace Chouette.Models
{
    public class SignUpModel
    {
        [Required(ErrorMessage ="Kullanıcı adı gereklidir!")]
        public string UserName { get; set; }

        [Required(ErrorMessage ="Lütfen isminizi giriniz!")]
        public string Name { get; set; }    
        
        [Required(ErrorMessage ="Lütfen soyisminizi giriniz!")]
        public string SurName { get; set; }

        [EmailAddress(ErrorMessage ="Lütfen email formatı giriniz!")]
        [Required(ErrorMessage = "Email gereklidir!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre gereklidir!")]
        public string Password { get; set; }

        [Compare("Password",ErrorMessage = "Parolalar eşleşmiyor!")]
        public string PasswordConfirm { get; set; }


        [Required(ErrorMessage = "Cinsiyet alanı gereklidir!")]
        public string Gender { get; set; }

        [Phone(ErrorMessage ="Lütfen geçerli bir telefon numarası giriniz!")]
        [Required(ErrorMessage ="Lütfen telefon numarası giriniz")]
        public string PhoneNumber { get; set; }

    }
}
