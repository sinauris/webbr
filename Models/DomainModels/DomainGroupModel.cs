namespace Webbr.Models.DomainModels
{
    public class DomainGroupModel
    {
        public string group_guid { get; set; } // objectGUID
        public string group_sid { get; set; } // objectSid
        public string group_dn { get; set; } // distinguishedName


        public string group_name { get; set; } // Имя группы
        public string group_login { get; set; } // Логин группы?
        public string group_description { get; set; } // Описание группы
        public string group_member { get; set; } // Пользователи\компьютеры данной группы
        public string group_memberOf { get; set; } // В каких группах находится

        public string group_change_datetime { get; set; } // Время последнего изменения группы
        public string group_create_datetime { get; set; } // Время создания группы
    }
}
