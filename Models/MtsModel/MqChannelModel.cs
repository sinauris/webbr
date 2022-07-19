namespace Webbr.Models.MtsModel
{
    public class MqChannelModel
    {
        public string mq_ip { get; set; }
        public string description { get; set; }
        public string queue_manager_name { get; set; }
        public string channel_name { get; set; }
        public string channel_status { get; set; }
        public string channel_state { get; set; }
        public string channel_connection_name { get; set; }
        public string channel_local_ip { get; set; }
        public string channel_rqm { get; set; }
        public string updated { get; set; }
    }
}