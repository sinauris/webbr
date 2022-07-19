using System.ComponentModel.DataAnnotations;

namespace Webbr.ViewModels.DutyViewModel
{
    public class UpdateDutyViewModel
    {
        [Required(ErrorMessage = "Поле обязателено для заполнения")]
        public int Id { get; set; }

        [StringLength(30, ErrorMessage = "Длина строки должна быть до 30 символов")]
        public string Stpinc { get; set; }

        [Required(ErrorMessage = "Поле обязателено для заполнения")]
        [RegularExpression(@"-?\d+(?:\.\d+)?", ErrorMessage = "Используйте только цифры и точку как разделитель")]
        [StringLength(7, ErrorMessage = "Длина строки должна быть до 7 символов")]
        public string Balance { get; set; }

        [RegularExpression(@"-?\d+(?:\.\d+)?", ErrorMessage = "Используйте только цифры и точку как разделитель")]
        [StringLength(7, ErrorMessage = "Длина строки должна быть до 7 символов")]
        public string Balance2 { get; set; }

/*        [Required(ErrorMessage = "Поле обязателено для заполнения")]
        [StringLength(200, ErrorMessage = "Длина строки должна быть до 200 символов")]
        public string Name { get; set; }*/

        [StringLength(500, ErrorMessage = "Длина строки должна быть до 500 символов")]
        public string Comment { get; set; }

        [Required(ErrorMessage = "Поле обязателено для заполнения")]
        [StringLength(50, ErrorMessage = "Длина строки должна быть до 50 символов")]
        public string Status { get; set; }
    }
}
