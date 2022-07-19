using System.ComponentModel.DataAnnotations;

namespace Webbr.ViewModels.DomainViewModel
{
    public class ResetPasswordDomainUserViewModel
    {
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(150, ErrorMessage = "Длина строки должна быть от {2} до {1} символов", MinimumLength = 3)]
        public string UserDn { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(50, ErrorMessage = "Длина строки должна быть от {2} до {1} символов", MinimumLength = 6)]
        public string AdminPassword { get; set; }
    }
}
