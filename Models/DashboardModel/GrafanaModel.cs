namespace Webbr.Models.DashboardModel
{
    public class GrafanaModel
    {
        public string name { get; set; }
        public string ip { get; set; }
        public string load { get; set; }
        public string load_treshold { get; set; }

        public string comment { get; set; }
        public string tag { get; set; }

        public int section { get; set; }
    }
}
