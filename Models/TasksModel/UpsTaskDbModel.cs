namespace Webbr.Models.TasksModel
{
    public class UpsTaskDbModel
    {
        public string ip { get; set; }
        public string name { get; set; }
        public int input_vol { get; set; }
        public int output_vol { get; set; }
        public int ups_load { get; set; }
        public int battery_temp { get; set; }
        public int battery_capacity { get; set; }
        public int battery_second { get; set; }
        public int battery_min_remaining { get; set; }

        public string snmp_public_string { get; set; }
        public string snmp_oid { get; set; }
        public string snmp_oid_input_voltage { get; set; }
        public string snmp_oid_output_voltage { get; set; }
        public string snmp_oid_output_percent_load { get; set; }
        public string snmp_oid_battery_temperature { get; set; }
        public string snmp_oid_estimated_charge_remaining { get; set; }
        public string snmp_oid_seconds_on_battery { get; set; }
        public string snmp_oid_estimated_minutes_remaining { get; set; }

        public string tooltip { get; set; }
//        public string tag { get; set; }
        public int placeid { get; set; }
        public string updated { get; set; }

        public int enable { get; set; }
    }
}
