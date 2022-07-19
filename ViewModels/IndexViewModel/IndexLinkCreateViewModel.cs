using System.ComponentModel.DataAnnotations;

namespace Webbr.ViewModels.IndexViewModel
{
    public class IndexLinkCreateViewModel
    {
        [Required] public string Link { get; set; }

        [Required] public string Description { get; set; }

        [Required] public string Column { get; set; }
    }
}
