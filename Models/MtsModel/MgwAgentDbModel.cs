namespace Webbr.Models.MtsModel
{
    public class MgwAgentDbModel
    {
        public string agent_name { get; set; }
        public string agent_status { get; set; }
        public string agent_ping { get; set; }
        public string agent_job { get; set; }
        public string agent_user { get; set; }
        public string agent_database { get; set; }
        public string agent_instance { get; set; }
        public string agent_start_time { get; set; }
        public string max_connections { get; set; }
        public string max_memory { get; set; }
        public string max_threads { get; set; }
        public string last_error_date { get; set; }
        public string last_error_time { get; set; }
        public string last_error_msg { get; set; }
        public string conntype { get; set; }
        public string service { get; set; }
        public string initfile { get; set; }
        public string comments { get; set; }
        public string updated { get; set; }
    }
}