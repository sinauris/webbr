using System.ComponentModel.DataAnnotations;

namespace Webbr.ViewModels.PortmapViewModel
{
    public class UpdatePortmapViewModel
    {
        [Required] public string Sw { get; set; }
        [Required] public string Swport { get; set; }
        public string Rm { get; set; }
        public string Ip { get; set; }
    }
}
