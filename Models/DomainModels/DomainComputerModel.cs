namespace Webbr.Models.DomainModels
{
    public class DomainComputerModel
    {
        public string computer_guid { get; set; } // objectGUID
        public string computer_sid { get; set; } // objectSid
        public string computer_dn { get; set; } // distinguishedName


        public string computer_ip { get; set; } // IP-адрес компьютера
        public string computer_mac { get; set; } // MAC-адрес компьютера
        public string computer_name { get; set; } // Имя компьютера
        public string computer_login { get; set; } // Логин того, кто авторизован на компьютере
        public string computer_groups { get; set; } // Группы, в которых компьютер состоит

        public string computer_os { get; set; } // Операционная система компьютера

        public string computer_power_datetime { get; set; } // Время включения компьютера
        public string computer_logon_datetime { get; set; } // Время последней авторизации на компьютере
        public string computer_create_datetime { get; set; } // Время добавления компьютера в домен
    }
}
