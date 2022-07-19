using System.ComponentModel.DataAnnotations;

namespace Webbr.ViewModels.DomainViewModel
{
    public class CreateDomainUserViewModel
    {
        [Required(ErrorMessage = "Поле {0} обязательно для заполнения")]
        public string CreatorPassword { get; set; }

        [Required(ErrorMessage = "Поле {0} обязательно для заполнения")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Поле {0} обязательно для заполнения")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "Поле {0} обязательно для заполнения")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Поле {0} обязательно для заполнения")]
        public string Password { get; set; }


        public string City { get; set; }

        [Required(ErrorMessage = "Поле {0} обязательно для заполнения")]
        public string Department { get; set; }

        [Required(ErrorMessage = "Поле {0} обязательно для заполнения")]
        public string Dn { get; set; }

        [Required(ErrorMessage = "Поле {0} обязательно для заполнения")]
        public string Title { get; set; } // Должность

        [Required(ErrorMessage = "Поле {0} обязательно для заполнения")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Поле {0} обязательно для заполнения")]
        public string ChangePasswordAtLogon { get; set; }

        [Required(ErrorMessage = "Поле {0} обязательно для заполнения")]
        public string PasswordExpires { get; set; }

        [Required(ErrorMessage = "Поле {0} обязательно для заполнения")]
        public string UserType { get; set; }

//        [Required(ErrorMessage = "Поле {0} обязательно для заполнения")]
//        public string CreateDfs { get; set; }
    }
}
