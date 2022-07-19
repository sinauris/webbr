using System.ComponentModel.DataAnnotations;

namespace Webbr.ViewModels.IndexViewModel
{
    public class CreateNewsViewModel
    {
        [Required]
        [StringLength(500, ErrorMessage = "Длина заголовка должна быть от {2} до {1} символов", MinimumLength = 3)]
        public string Title { get; set; }

        [Required]
        [StringLength(100000, ErrorMessage = "Длина новости должна быть от {2} до {1} символов", MinimumLength = 3)]
        public string Body { get; set; }

//        [Required]
//        public string Whocansee { get; set; }

//        [Required]
//        public string Important { get; set; }
    }
}
