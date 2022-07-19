using System;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;

namespace Webbr.Models.PortmapModel
{
    public class MainPortmapModel
    {
        private string _updateTime;
        private string _nau_skills;
        private string _nau_projects;

        public string mac { get; set; }
        public string rm { get; set; }
        public string ip { get; set; }

        public string sw { get; set; }
        public int swport { get; set; }
        public int vlan { get; set; }

        public string image { get; set; }

        public string nau_uuid { get; set; }
        public string nau_login { get; set; }
        public string nau_first_name { get; set; }
        public string nau_middle_name { get; set; }
        public string nau_last_name { get; set; }
        public string nau_department { get; set; }
        public string nau_location_name { get; set; }
        public string nau_profile_name { get; set; }
        public string nau_creation_time { get; set; }
        public string nau_roles { get; set; }
//        public string nau_projects { get; set; }

        public string win_user_username { get; set; }
        public string win_user_login { get; set; }
        public string win_user_department { get; set; }
        public string win_user_description { get; set; }
        public string win_user_city { get; set; }
        public string win_computer_name { get; set; }
        public string win_computer_logon_datetime { get; set; }
        public string win_computer_power_datetime { get; set; }
        
        public string inv_motherboard { get; set; }
        public string inv_cpu { get; set; }
        public string inv_ram { get; set; }
        public string inv_hdd { get; set; }
        public string inv_monitor { get; set; }

        public string typeos { get; set; }
        public string typeip { get; set; }

        public string onlinestatus { get; set; }

        public string updatetime
        {
            get => int.TryParse(_updateTime, out var variable)
                ? new DateTime(1970, 1, 1).AddSeconds(variable).ToLocalTime().ToString("O", CultureInfo.CurrentCulture)
                : _updateTime;
            set => _updateTime = value;
        }
        
        public string nau_projects
        {
            get => _nau_projects;
            set
            {
                if (string.IsNullOrEmpty(value)) _nau_projects = value;
                else
                {
                    try
                    {                       
                        var list = value.Split("; ").Select(s => s.Split(" - ")).Select(z =>
                        {
//                            return new Project {Name = splitArr[0], Object = splitArr[1]};
                            
                            if (z.Length == 2) return new Project { Name = z[0], Object = z[1] };
                            
                            return new Project();
                        }).ToList();

                        
                        _nau_projects = JsonConvert.SerializeObject(list);
//                        var nauProjList = _nau_projects.ToList<Project>();
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }
        
        public string nau_skills
        {
            get => _nau_skills;
            set
            {
                if (string.IsNullOrEmpty(value)) _nau_skills = value;
                else
                {
                    try
                    {
                        var arr = value.Split("; ");
                        var list = arr.Select(s => s.Split(" - ")).Select(z =>
                        {
                            if (z.Length == 2) return new Skill { Name = z[0], Type = z[1] == "IN" ? "incoming_project" : "outcoming_project" };
                            
                            return new Skill();
                        }).ToList();
                        _nau_skills = JsonConvert.SerializeObject(list);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{value} |||| {ex.Message}");
                    }
                }
            }
        }
        
        public int placeid { get; set; }
    }
    
    public class Project
    {
        public string Name { get; set; }
        public string Object { get; set; }
    }
    public class Skill
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}