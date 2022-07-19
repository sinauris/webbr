namespace Webbr.Models.MtsModel
{
    public class MqQueueModel
    {
        public string mq_ip { get; set; }
        public string description { get; set; }
        public string queue_manager_name { get; set; }
        public string queue_name { get; set; }
        public int queue_depth { get; set; }
        public string updated { get; set; }
    }
}