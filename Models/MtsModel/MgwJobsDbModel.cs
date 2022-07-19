namespace Webbr.Models.MtsModel
{
    public class MgwJobsDbModel
    {
        public string agent_name { get; set; }
        public string enabled { get; set; }
        public string source { get; set; }
        public string propagated_msgs { get; set; }
        public string status { get; set; }
        public string systime { get; set; }
        public string job_name { get; set; }
        public string propagation_type { get; set; }
        public string destination { get; set; }
        public string link_name { get; set; }
        public string last_error_msg { get; set; }
        public string last_error_date { get; set; }
        public string updated { get; set; }
    }
}