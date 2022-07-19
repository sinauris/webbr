using ServiceStack.DataAnnotations;

namespace Webbr.ViewModels.ConfigurationViewModel.PlacesViewModel
{
    public class DeleteCitiesViewModel
    {
        [Required] public int placeid { get; set; }
    }
}
