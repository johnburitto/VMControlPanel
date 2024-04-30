using Bot.Localization;
using Core.Entities;

namespace Bot.Utilities
{
    public static class NoAuthCommands
    {
        public static Cultures Culture { get; set; } = Cultures.En;
        public static List<string> Commands => GetCommands();

        private static List<string> GetCommands()
        {
            return ["/start", LocalizationManager.GetString("Register", Culture), LocalizationManager.GetString("Login", Culture), $"🚪 {LocalizationManager.GetString("Logout", Culture)}"];
        }
    }
}
