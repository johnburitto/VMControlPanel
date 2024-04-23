using Bot.Localization;
using Core.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Utilities
{
    public static class Keyboards
    {
        public static Cultures Culture { get; set; } = Cultures.En;
        public static ReplyKeyboardMarkup? Null = null;
        public static ReplyKeyboardMarkup StartKeyboard => CreateStartKeyboard();
        public static ReplyKeyboardMarkup CancelKeyboard => CreateCancelKeyboard();
        public static ReplyKeyboardMarkup VMActionKeyboard => CreateVMActionKeyboard();
        private static ReplyKeyboardMarkup CreateStartKeyboard()
        {
            return new ReplyKeyboardMarkup([
                new KeyboardButton[] { LocalizationManager.GetString("Register", Culture), LocalizationManager.GetString("Login", Culture) }
            ])
            {
                ResizeKeyboard = true
            };
        }
        private static ReplyKeyboardMarkup CreateCancelKeyboard()
        {
            return new([
                new KeyboardButton[] { $"❌ {LocalizationManager.GetString("ToCancel", Culture)}" }
            ])
            {
                ResizeKeyboard = true
            };
        }

        private static ReplyKeyboardMarkup CreateVMActionKeyboard()
        {
            return new([
                new KeyboardButton[] { LocalizationManager.GetString("ExecuteCommands", Culture) },
                new KeyboardButton[] { LocalizationManager.GetString("CreateDirectory", Culture), LocalizationManager.GetString("DeleteDirectory", Culture) },
                new KeyboardButton[] { LocalizationManager.GetString("DownloadFile", Culture), LocalizationManager.GetString("UploadFile", Culture) },
                new KeyboardButton[] { LocalizationManager.GetString("Metrics", Culture), $"🚪 {LocalizationManager.GetString("Logout", Culture)}" }
            ])
            {
                ResizeKeyboard = true
            };
        }
    }
}
