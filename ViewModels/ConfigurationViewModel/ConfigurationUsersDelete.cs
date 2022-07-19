using System.ComponentModel.DataAnnotations;

namespace Webbr.ViewModels.ConfigurationViewModel
{
    public class ConfigurationUsersDelete
    {
        [Required] public int AdLogin { get; set; }
    }
}
