using Core.Entities;
using Core.Localization.Languages;
using System.Globalization;

namespace Bot.Localization
{
    public static class LocalizationManager
    {
        public static string GetString(string key, Cultures culture = Cultures.En)
        {
            Strings.Culture = CultureInfo.GetCultureInfo(culture.ToString());

            return Strings.ResourceManager.GetString(key) ?? Strings.InvalidKey;
        }
    }
}
