using Bot.Localization;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Utilities
{
    public static class Keyboards
    {
        public static ReplyKeyboardMarkup? Null = null;
        public static ReplyKeyboardMarkup StartKeyboard = new ReplyKeyboardMarkup([
            new KeyboardButton[] { LocalizationManager.GetString("Register"), LocalizationManager.GetString("Login") }
        ])
        {
            ResizeKeyboard = true,
            IsPersistent = true,
        };
        public static ReplyKeyboardMarkup CancelKeyboard = new([
            new KeyboardButton[] { $"❌ {LocalizationManager.GetString("ToCancel")}" }
        ])
        {
            ResizeKeyboard = true,
            IsPersistent = true,
        };
        public static ReplyKeyboardMarkup VMActionKeyboard = new([
            new KeyboardButton[] { LocalizationManager.GetString("ExecuteCommands") },
            new KeyboardButton[] { LocalizationManager.GetString("CreateDirectory"), LocalizationManager.GetString("DeleteDirectory") },
            new KeyboardButton[] { LocalizationManager.GetString("DownloadFile"), LocalizationManager.GetString("UploadFile") },
            new KeyboardButton[] { LocalizationManager.GetString("Metrics"), $"🚪 {LocalizationManager.GetString("Logout")}" }
        ])
        {
            ResizeKeyboard = true,
            IsPersistent = true,
        };
    }
}
