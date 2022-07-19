using System.ComponentModel.DataAnnotations;

namespace Webbr.ViewModels.DomainViewModel
{
    public class CheckDomainUserExistViewModel
    {
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(20, ErrorMessage = "Длина строки должна быть от {2} до {1} символов", MinimumLength = 3)]
        //[RegularExpression(@"-?[a-zA-Z]+(?:\.[a-zA-Z]+)?", ErrorMessage = "Допустимые символы: A-z и точка")]
        //[RegularExpression(@"-?[a-zA-Z0-9]+(?:\.[a-zA-Z0-9]+)?", ErrorMessage = "Допустимые символы: A-z, 0-9 и разделитель точка")]
        //[RegularExpression(@"-?[a-zA-Z0-9]+(?:\.[a-zA-Z0-9]+)?(?:\.[a-zA-Z0-9]+)?", ErrorMessage = "Допустимые символы: [A-z, 0-9, _, .]")]
        [RegularExpression(@"^[A-Za-z0-9\\\._-]{3,}$", ErrorMessage = "Допустимые символы: [A-z, 0-9, _, .]")]
        public string Login { get; set; }
    }
}
