using System.ComponentModel.DataAnnotations;

namespace Webbr.ViewModels.IndexViewModel
{
    public class IndexSectionCreateViewModel
    {
        [Required] public string Section_name { get; set; }
    }
}
