using ServiceStack.DataAnnotations;

namespace Webbr.ViewModels.ConfigurationViewModel.PlacesViewModel
{
    public class CreatePlacesViewModel
    {
        [Required] public string place_description { get; set; }
        [Required] public string place_short_description { get; set; }
        public string dhcp_server { get; set; }
        public string dhcp_server_command { get; set; }
        public string naumen_server { get; set; }
        public string comment { get; set; }
    }
}
