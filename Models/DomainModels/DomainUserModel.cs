namespace Webbr.Models.DomainModels
{
    public class DomainUserModel
    {
        public string user_guid { get; set; } // objectGUID
        public string user_dn { get; set; } // distinguishedName

        public string user_firstname { get; set; } // Имя пользователя
        public string user_lastname { get; set; } // Фамилия пользователя
        public string user_username { get; set; } // ФИО пользователя
        public string user_login { get; set; } // Логин пользователя
        public string user_mail { get; set; } // Email пользователя

        public string user_department { get; set; } // Отдел пользователя
        public string user_description { get; set; } // Должность пользователя
        public string user_city { get; set; } // Город пользователя

        public string user_groups { get; set; } // Группы, в которых состоит пользователь
        public string user_mobile_phone { get; set; } // Сотовый телефон пользователя
        public string user_naumen_phone { get; set; } // Номер Naumen пользователя
        public string user_proxy { get; set; } // Proxy-адреса, прописанные пользователю
        public string user_password_last_set { get; set; } // Когда в последний раз была смена пароля

        public string user_ip { get; set; } // Последний IP-адрес компьютера, за которым работал пользователь
        public string user_mac { get; set; } // Последний MAC-адрес компьютера, за которым работал пользователь
        public string user_computer { get; set; } // Последний компьютер, за которым работал пользователь

        public string user_logon_datetime { get; set; } // Последний вход пользователя в домен
        public string user_create_datetime { get; set; } // Дата создания пользователя в домене

        public int user_account_type { get; set; } // Тип аккаунта, число

        public string
            user_disabled
        {
            get;
            set;
        } // Состояние аккаунта пользователя                          | normal_account/disabled_account

        public string
            user_password_expires
        {
            get;
            set;
        } // Требуется ли смена пароля по истечении времени   | password_not_expires/password_expires

        public string
            user_password_required
        {
            get;
            set;
        } // Требуется ли пароль                             | password_required/password_not_required

        public string user_encryption { get; set; } // Используемое шифрование
    }
}
