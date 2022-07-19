using System.ComponentModel.DataAnnotations;

namespace Webbr.ViewModels.CommandsViewModel
{
    public class SshCommandViewModel
    {
        [Required] public string Rm { get; set; }

        [Required] public string Ip { get; set; }
    }
}
