namespace Webbr.Models.ConfigurationModel
{
    public class ConfigurationCommutatorModel
    {
        public int id { get; set; }
        public string ip { get; set; }
        public string port { get; set; }
        public string port_offset { get; set; }
        public string snmp_public_string { get; set; }
        public string snmp_oid { get; set; }
        public string comment { get; set; }
        public int enable { get; set; }
    }
}
