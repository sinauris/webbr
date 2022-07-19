namespace Webbr.Models.TasksModel
{
    public class GrafanaTaskDbModel
    {
        public string Name { get; set; }
        public string Ip { get; set; }
        public string Cpu { get; set; }
        public string Cpu_treshold { get; set; }
        public string Ram { get; set; }
        public string Ram_treshold { get; set; }
        public string Comment { get; set; }
        public string Tag { get; set; }
        public string Online { get; set; }
        public string Updated { get; set; }
    }
}
