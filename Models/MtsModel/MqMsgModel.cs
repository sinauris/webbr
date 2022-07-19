namespace Webbr.Models.MtsModel
{
    public class MqMsgModel
    {
        public string mq_ip { get; set; }
        public string queue_manager_name { get; set; }
        public string queue_name { get; set; }
        public string msg_request_id { get; set; }
        public int msg_length { get; set; }
        public string msg_put_datetime { get; set; }
        public string in_queue { get; set; }
        public string updated { get; set; }
    }
}