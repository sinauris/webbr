namespace Webbr.Models.PersonalModel
{
    public class PersonalModel
    {
        public string ad_guid { get; set; }
        public string ad_login { get; set; } // Primary Unique Key
        public string ad_name { get; set; }
        public string ad_department { get; set; }
        public string ad_place { get; set; }
        public string webbr_role { get; set; }
        public string webbr_banned { get; set; }
        public string webbr_register_datetime { get; set; }
        public string webbr_auth_datetime { get; set; }
    }
}
