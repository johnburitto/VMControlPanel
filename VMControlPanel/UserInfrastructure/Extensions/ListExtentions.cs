using Core.Entities;

namespace UserInfrastructure.Extensions
{
    public static class ListExtensions
    {
        public static string ToStringList(this IEnumerable<User> users)
        {
            var account = string.Join("\n", users.Select((user, index) => $"{index + 1}. <i>{user.UserName}</i>"));

            return account == string.Empty ? "У вас немає зареєстрованих акаунтів" : $"Ваші акаунти:\n{account}";
        }
    }
}
