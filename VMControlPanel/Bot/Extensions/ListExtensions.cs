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

            buttons.Add([new KeyboardButton("➕ Додати нову машину")]);

            return new(buttons) { ResizeKeyboard = true };
        }
    }
}
