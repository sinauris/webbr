namespace Webbr.Models.ConfigurationModel
{
    public class ConfigurationDashboardServerModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string ip { get; set; }
        public string cpu { get; set; }
        public string cpu_treshold { get; set; }
        public string ram { get; set; }
        public string ram_treshold { get; set; }
        public string comment { get; set; }
        public string tag { get; set; }
        public string updated { get; set; }
        public string online { get; set; }
    }
}
