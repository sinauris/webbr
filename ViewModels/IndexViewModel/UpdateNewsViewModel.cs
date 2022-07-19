using System.ComponentModel.DataAnnotations;

namespace Webbr.ViewModels.IndexViewModel
{
    public class UpdateNewsViewModel
    {
        [Required] public int Id { get; set; }

        [Required] public string Title { get; set; }

        [Required] public string Body { get; set; }

//        [Required]
//        public string Publish { get; set; }
    }
}
