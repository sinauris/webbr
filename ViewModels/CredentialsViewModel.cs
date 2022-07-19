using System.ComponentModel.DataAnnotations;

namespace Webbr.ViewModels
{
    public class CredentialsViewModel
    {
        [Required(ErrorMessage = "Поле Логин обязателено для заполнения")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Поле Пароль обязателено для заполнения")]
        public string Password { get; set; }
    }
}
