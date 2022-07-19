using System.ComponentModel.DataAnnotations;

namespace Webbr.ViewModels.ConfigurationViewModel
{
    public class ConfigurationCommutatorSwitchState
    {
        [Required] public int Id { get; set; }

        [Required] public int Enable { get; set; }
    }
}
