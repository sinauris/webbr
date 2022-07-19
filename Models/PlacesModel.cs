namespace Webbr.Models
{
    internal class PlacesModel
    {
        public int placeid { get; set; }
        public string place_short_description { get; set; }
        public string place_description { get; set; }
        public string dhcp_server { get; set; }
        public string dhcp_server_command { get; set; }
        public string naumen_server { get; set; }
        public int place_enabled { get; set; }
    }
}
