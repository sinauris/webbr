using System.ComponentModel.DataAnnotations;

namespace Webbr.ViewModels.IndexViewModel
{
    public class DeleteNewsViewModel
    {
        [Required] public int Id { get; set; }
    }
}
