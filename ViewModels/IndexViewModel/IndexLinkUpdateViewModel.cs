using System.ComponentModel.DataAnnotations;

namespace Webbr.ViewModels.IndexViewModel
{
    public class IndexLinkUpdateViewModel
    {
        [Required] public int Id { get; set; }

        [Required] public string Link { get; set; }

        [Required] public string Description { get; set; }

        [Required] public int Column { get; set; }
    }
}
