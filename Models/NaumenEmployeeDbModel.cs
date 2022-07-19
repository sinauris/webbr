using System;
using System.Globalization;

namespace Webbr.Models
{
    public class NaumenEmployeeDbModel
    {
        private string _removed;
        private string _creation_time;
        
        public string id_employee { get; set; }
        
//        public string name_employee { get; set; }
        
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public string login { get; set; }
        public string department { get; set; }
        public string location_name { get; set; }
        public string profile_name { get; set; }
        public string roles { get; set; }
        public string skills { get; set; }
        public string projects { get; set; }

        public string creation_time
        {
            get => _creation_time;
            set
            {
                if (!string.IsNullOrEmpty(value)) _creation_time = DateTime.ParseExact(value, "dd.MM.yyyy H:mm:ss", CultureInfo.InvariantCulture).ToString("O");
                else _creation_time = value;
            }
        }

        public string removed
        {
            get => _removed;
            set => _removed = value == "0" ? "employee_active" : "employee_removed";
        }
    }
}