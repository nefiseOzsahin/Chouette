using Microsoft.AspNetCore.Identity;

namespace Chouette.CustomDescriber
{
    public class CustomErrorDescriber:IdentityErrorDescriber
    {

        public override IdentityError PasswordTooShort(int length)
        {
            return new()
            {
                Code= "PasswordTooShort",
                Description=$" Parola en az {length} karakter olmalıdır!"
            };
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new()
            {
                Code= "PasswordRequiresNonAlphanumeric",
                Description="Parola en az bir alpha numerik(! vb) karakter içermelidir!"
            };
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return new()
            {
                Code = "DuplicateUserName",
                Description = $"{userName} zaten alınmıştır!"
            };
        }

        public override IdentityError InvalidUserName(string userName)
        {
            return new()
            {
                Code = "InvalidUserName",
                Description = $"{userName} kullanıcı adı geçersizdir! Sadece harf ve sayı içerebilir!"
            };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new()
            {
                Code = "PasswordRequiresLower",
                Description = "Parola en az 1 küçük  harf içermelidir!"
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new()
            {
                Code = "PasswordRequiresUpper",
                Description = "Parola en az 1 büyük harf içermelidir!"
            };
        }
    }
}
