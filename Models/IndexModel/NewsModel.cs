using System.ComponentModel.DataAnnotations;

namespace Webbr.Models.IndexModel
{
    public class NewsModel
    {
        [Required] public string Id { get; set; }

        public string Title { get; set; }
        public string Body { get; set; }
        public string Author { get; set; }
        public string AdLogin { get; set; }
        public string Createdtime { get; set; }
        public string Changetime { get; set; }
        public string Changelogin { get; set; }
    }
}
