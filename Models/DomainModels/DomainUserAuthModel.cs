namespace Webbr.Models.DomainModels
{
    public class DomainUserAuthModel
    {
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public string Description { get; set; }
        public string Department { get; set; }
        public string L { get; set; }
        public string Guid { get; set; }
        public string Theme { get; set; }

        public string ExceptionKey { get; set; }
        public string ExceptionError { get; set; }
    }
}