using System;
using System.Text.RegularExpressions;

namespace Webbr.Models.ServerModel
{
    public class VirtualServerModel
    {
        private string _vmUptime;
        private string _vmGroup;

        public string vmUuid { get; set; }
        public string hypervisorIp { get; set; }
        public string vmCluster { get; set; }
        public string vmName { get; set; }
        public string vmGuestos { get; set; }
        public string vmState { get; set; }
        public string vmHost { get; set; }
        public string vmPath { get; set; }
        public string vmAnnotation { get; set; }

        public string vmUptime
        {
            get
            {
                if (_vmUptime == "0") return "-";

                var t = TimeSpan.FromSeconds(Convert.ToDouble(_vmUptime));
                return t.Days + "д " + t.Hours + "ч " + t.Minutes + "м";
            }
            set => _vmUptime = value;
        }
        
        public string vmGroup
        {
            get => new Regex("\".+\"", RegexOptions.IgnoreCase).Match(vmAnnotation).Value.Replace("\"","");
            set => _vmGroup = value;
        }

        public string vmIpaddress { get; set; }
        public string vmDnsname { get; set; }
        public string vmCpu { get; set; }
        public string vmMemory { get; set; }
        public string vmHdd { get; set; }
        public string vmNet { get; set; }
        public string vmToolsstatus { get; set; }
        public string vmToolsversion { get; set; }
        public string attrMaster { get; set; }
        public string attrReplicalocation { get; set; }
        public string attrRto { get; set; }
        public string attrRpo { get; set; }
        public string attrClusternodes { get; set; }
        public string hypervisorType { get; set; }
    }
}