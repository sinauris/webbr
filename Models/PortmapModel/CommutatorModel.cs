namespace Webbr.Models.PortmapModel
{
    public class CommutatorModel
    {
        public string ip { get; set; }
        public string port { get; set; }
        public int port_offset { get; set; }
        public string snmp_public_string { get; set; }
        public string snmp_oid { get; set; }
    }
}
