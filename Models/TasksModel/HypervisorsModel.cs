namespace Webbr.Models.TasksModel
{
    public class HypervisorsModel
    {
        public int id { get; set; }
        public string hypervisor { get; set; }
        public string url { get; set; }
        public string ip { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string comment { get; set; }
        public int enable { get; set; }
    }
}