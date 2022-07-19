using System.ComponentModel.DataAnnotations;

namespace Webbr.ViewModels.ConfigurationViewModel
{
    public class ConfigurationUsersRole
    {
        [Required] public string AdLogin { get; set; }

        [Required] public string WebbrRole { get; set; }
    }
}
