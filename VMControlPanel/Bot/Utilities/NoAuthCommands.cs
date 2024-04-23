using Bot.Localization;

namespace Bot.Utilities
{
    public static class NoAuthCommands
    {
        public static List<string> Commands = [ "/start", LocalizationManager.GetString("Register"), LocalizationManager.GetString("Login"), $"🚪 {LocalizationManager.GetString("Logout")}"];
    }
}
