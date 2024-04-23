using Bot.Localization.Languages;
using Core.Entities;
using System.Globalization;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Localization
{
    public static class LocalizationManager
    {
        public static string GetString(string key, Cultures culture = Cultures.En)
        {
            Strings.Culture = CultureInfo.GetCultureInfo(culture.ToString());

            return Strings.ResourceManager.GetString(key, Strings.Culture) ?? Strings.InvalidKey;
        }

        public static string GetStringRaw(string key, CultureInfo culture)
        {
            return Strings.ResourceManager.GetString(key, culture) ?? Strings.InvalidKey;
        }

        public static List<string> GetLanguages()
        {
            var enums = Enum.GetNames(typeof(Cultures));

            return enums.Select(_ => GetStringRaw(_, CultureInfo.GetCultureInfo(_))).ToList();
        }

        public static ReplyKeyboardMarkup GetLanguagesKeyboard()
        {
            var languages = GetLanguages();

            return new(languages.Select(_ => new KeyboardButton[] { new KeyboardButton(_) }))
            {
                ResizeKeyboard = true
            };
        }

        public static Cultures GetLanguage(string? code)
        {
            if (code == null)
            {
                return Cultures.En;
            }

            var languages = Enum.GetNames(typeof(Cultures)).Select(_ => _.ToLower()).ToList();
            var language = languages.IndexOf(code);

            return language == -1 ? Cultures.En : (Cultures)language;
        }

        public static Cultures? DetermineLanguage(string? language)
        {
            if (language == null)
            {
                return null;
            }

            if (language.Contains("🇺🇦"))
            {
                return Cultures.Uk;
            }
            else if (language.Contains("🇬🇧"))
            {
                return Cultures.En;
            }

            return null;
        }
    }
}
