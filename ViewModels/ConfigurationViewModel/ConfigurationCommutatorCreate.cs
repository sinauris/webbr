using System.ComponentModel.DataAnnotations;

namespace Webbr.ViewModels.ConfigurationViewModel
{
    public class ConfigurationCommutatorCreate
    {
        [Required] public string ip { get; set; }
        [Required] public string port { get; set; }
        public string port_offset { get; set; }
        [Required] public string snmp_public_string { get; set; }
        [Required] public string snmp_oid { get; set; }
        [Required] public string comment { get; set; }
        [Required] public int placeid { get; set; }
    }
}
