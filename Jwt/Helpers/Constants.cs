namespace Webbr.Jwt.Helpers
{
    public static class Constants
    {
        public static class JwtClaimIdentifiers
        {
            public const string Id = "id"; // ID

            public const string Din = "din"; // Display Name

            public const string Rol = "rol"; // Role

            public const string Plc = "plc"; // Place

            public const string Thm = "thm"; // Theme
        }

        public static class JwtPolicy
        {
            public const string Guest = "guest"; // Доступ только к Дашборду
            
            public const string Basic = "basic"; // Для обычных аккаунтов всех отделов (ОТП, ОИТ, ОАОД)

            public const string OtpOnly = "otponly"; // Только для ОТП

            public const string Mc = "mc"; // Для МС (инвентаризация)

            public const string Rg = "rg"; // Для РГ отдела ОТП

            public const string Admin = "admin"; // Для административных аккаунтов
        }

        public static class JwtClaims
        {
            // Level 4
            public const string Guest = "guest"; // Для самых маленьких
            
            // Level 3
            public const string Oit = "oit"; // Для ОИТ
            public const string Oaod = "oaod"; // Для ОАОД
            public const string Otp = "otp"; // Для ОТП

            // Level 2
            public const string Mc = "mc"; // Для МС (инвентаризация)

            // Level 1
            public const string Rg = "rg"; // Для РГ отдела ОТП

            // Level 0
            public const string Admin = "admin"; // Для административных аккаунтов
        }

        public static class WebbrTheme
        {
            public const string Light = "light"; // Светлая тема, по умолчанию
            public const string Dark = "dark"; // Тёмная тема
        }
    }
}