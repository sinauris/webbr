using System.ComponentModel.DataAnnotations;

namespace Webbr.ViewModels.ConfigurationViewModel
{
    public class ConfigurationUsersBanned
    {
        [Required] public string AdLogin { get; set; }

        [Required] public int WebbrBanned { get; set; }
    }
}
