using Bot.HttpInfrastructure;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Utilities
{
    public static class Keyboards
    {
        public static ReplyKeyboardMarkup? Null = null;
        public static ReplyKeyboardMarkup StartKeyboar = new ReplyKeyboardMarkup([
            new KeyboardButton[] { "Створити акаунт", "Увійти в акаунт" }
        ])
        {
            ResizeKeyboard = true
        };
        public static ReplyKeyboardMarkup CancelKeyboard = new([
            new KeyboardButton[] { "❌ Відмінити" }
        ])
        {
            ResizeKeyboard = true
        };
        public static ReplyKeyboardMarkup VMActionKeyboard = new([
            new KeyboardButton[] { "Виконувати команди" },
            new KeyboardButton[] { "Створити директорію", "Видалити директорію" },
            new KeyboardButton[] { "Завантажити файл", "Вивантажити файл" }
        ])
        {
            ResizeKeyboard = true
        };
    }
}
