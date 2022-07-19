using System.ComponentModel.DataAnnotations;

namespace Webbr.ViewModels.PortmapViewModel
{
    public class DeletePortmapViewModel
    {
        [Required] public string Sw { get; set; }
        [Required] public string Swport { get; set; }
    }
}
