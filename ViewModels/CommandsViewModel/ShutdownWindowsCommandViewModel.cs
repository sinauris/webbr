using System.ComponentModel.DataAnnotations;

namespace Webbr.ViewModels.CommandsViewModel
{
    public class ShutdownWindowsCommandViewModel
    {
        public string Rm { get; set; }

        [Required] public string Ip { get; set; }
    }
}
