using Bot.Localization.Languages;
using Core.Entities;
using System.Globalization;

namespace Bot.Localization
{
    public static class LocalizationManager
    {
        public static string GetString(string key, Cultures culture = Cultures.En)
        {
            Strings.Culture = CultureInfo.GetCultureInfo(culture.ToString());

            return Strings.ResourceManager.GetString(key, Strings.Culture) ?? Strings.InvalidKey;
        }
    }
}
