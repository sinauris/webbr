using System.ComponentModel.DataAnnotations;

namespace Webbr.ViewModels.IndexViewModel
{
    public class IndexSectionUpdateViewModel
    {
        [Required] public int Id { get; set; }

        [Required] public string Section_name { get; set; }
    }
}
