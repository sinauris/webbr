using System.ComponentModel.DataAnnotations;

namespace Webbr.ViewModels.CommandsViewModel
{
    public class ShutdownLinuxCommandViewModel
    {
        public string Rm { get; set; }

        [Required] public string Ip { get; set; }
    }
}
