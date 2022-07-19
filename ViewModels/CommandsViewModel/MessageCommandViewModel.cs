using System.ComponentModel.DataAnnotations;

namespace Webbr.ViewModels.CommandsViewModel
{
    public class MessageCommandViewModel
    {
        public string Rm { get; set; }

        [Required] public string Ip { get; set; }

        [Required] public string Title { get; set; }

        [Required] public string Message { get; set; }
    }
}
