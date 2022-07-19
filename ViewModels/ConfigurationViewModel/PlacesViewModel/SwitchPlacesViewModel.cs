using ServiceStack.DataAnnotations;

namespace Webbr.ViewModels.ConfigurationViewModel.PlacesViewModel
{
    public class SwitchPlacesViewModel
    {
        [Required] public int placeid { get; set; }
        [Required] public int place_enabled { get; set; }
    }
}