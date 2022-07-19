using System.ComponentModel.DataAnnotations;

namespace Webbr.ViewModels.DutyViewModel
{
    public class CreateDutyViewModel
    {
        [Required(ErrorMessage = "Поле обязателено для заполнения")]
        [RegularExpression(@"-?\d+(?:\.\d+)?", ErrorMessage = "Используйте только цифры и точку как разделитель")]
        [StringLength(7, ErrorMessage = "Длина строки должна быть до 7 символов")]
        public string Balance { get; set; }

        [RegularExpression(@"-?\d+(?:\.\d+)?", ErrorMessage = "Используйте только цифры и точку как разделитель")]
        [StringLength(7, ErrorMessage = "Длина строки должна быть до 7 символов")]
        public string Balance2 { get; set; }

        [StringLength(30, ErrorMessage = "Длина строки должна быть до 30 символов")]
        public string Stpinc { get; set; }

        [StringLength(200, ErrorMessage = "Длина строки должна быть до 200 символов")]
        public string Comment { get; set; }
    }
}
