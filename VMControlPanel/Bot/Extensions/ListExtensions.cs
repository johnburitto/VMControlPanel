using Bot.Localization;
using Core.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Extensions
{
    public static class ListExtensions
    {
        public static ReplyKeyboardMarkup ToKeyboard(this List<VirtualMachine> virtualMachines)
        {
            var buttons = virtualMachines.Select((_, i) => new { _.Name, i })
                                         .GroupBy(_ => _.i / 3)
                                         .Select(_ => _.Select(_ => new KeyboardButton($"{_.Name}")))
                                         .ToList();

            buttons.Add([new KeyboardButton($"➕ {LocalizationManager.GetString("AddNewMachine")}")]);

            return new(buttons) { ResizeKeyboard = true };
        }

        public static string ToStringList(this IEnumerable<User> users)
        {
            var account = string.Join("\n", users.Select((user, index) => $"{index + 1}. <i>{user.UserName}</i>"));

            return account == string.Empty ? $"{LocalizationManager.GetString("YouDontHaveAccounts")}" : $"{LocalizationManager.GetString("YourAccounts")}:\n{account}";
        }
    }
}
