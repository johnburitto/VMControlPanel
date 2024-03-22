<mark style="background: #FFF3A3A6;">ToDo:</mark>
- [x] <mark style="background: #BBFABBA6;">Створити команду для вибору віртуальної машини, із якою будемо взаємодіяти ✅ 2024-03-22</mark>
- [x] <mark style="background: #BBFABBA6;">Створити сервіс для SSH підключення до віртуальної машини ✅ 2024-03-22</mark>
- [x] <mark style="background: #BBFABBA6;">Додати в сервіс метод для виконання "не sudo" команд ✅ 2024-03-22</mark>
- [ ] Додати в сервіс метод для виконання "sudo" команд
- [ ] Створити контролер для SSHRequestService
- [ ] Створити команду для виконання команд через SSH

## Створити команду для вибору віртуальної машини, із якою будемо взаємодіяти
Логіка команди вибору віртуальної машини для взаємодії із нею:
1. В користувача є клавіатура з усіма віртуальними машинами, що належать його акаунту
2. Користувач натискає на кнопку машини, із якою хоче взаємодіяти
3. Система витягує цю машину із бази даних
4. Система кешує її
5. Користувачу виводиться повідомлення, що він взаємодіє саме із необхідною машиною

Для того, щоб зробити зручну та універсальну команду вибору віртуальної машини було необхідно, щоб команда реагувала тільки на назви віртуальних машин користувача. Бо саме ці назви надсилаються, коли натискаєш на кнопку на клавіатурі. Для цього трішки було переписано метод TryExecuteAsync:
```CSharp
public override async Task TryExecuteAsync(ITelegramBotClient client, Message? message)
{
    Names = await RequestClient.GetUserVirtualMachinesNamesAsync(message!.Chat.Id);

    await base.TryExecuteAsync(client, message);
}
```

Як видно в коді вище спочатку у властивість із назвами, на які реагує команда даються назви віртуальних машин користувача за допомогою методу RequestClient:
```CSharp
public static async Task<List<string>> GetUserVirtualMachinesNamesAsync(long telegramId)
{
    var response = await Client!.GetAsync($"https://localhost:8081/api/Cache/{telegramId}_current_user_id");
    var userId = await response.Content.ReadAsStringAsync();
    var virtualMachinesResponse = await Client!.GetAsync($"https://localhost:8081/api/VirtualMachine/{userId}/all");

    return JsonConvert.DeserializeObject<List<VirtualMachine>>(await virtualMachinesResponse.Content.ReadAsStringAsync())?.Select(_ => _.Name ?? "").ToList() ?? [];
}
```

Після чого вже виконується звичайна логіка методу TryExecuteAsync.

Далі команда за тим іменем, що отримала в тексті та ідентифікатором користувача витягує із бази об'єкт віртуально машини, кешує його та виводить повідомлення користувачу:
```CSharp
public override async Task ExecuteAsync(ITelegramBotClient client, Message? message)
{
    var virtualMachine = await RequestClient.GetVirtualMachineByUserIdAndVMNameAsync(message!.Chat.Id, message?.Text);

    await RequestClient.CacheAsync($"{message!.Chat.Id}_vm", JsonConvert.SerializeObject(virtualMachine), 1f);
    await client.SendTextMessageAsync(message!.Chat.Id, $"Ви обрали для взаємодії віртуальну машину {virtualMachine?.Name}", parseMode: ParseMode.Html);
}
```

Для витягу машини використовується метод RequestClient:
```CSharp
public static async Task<VirtualMachine?> GetVirtualMachineByUserIdAndVMNameAsync(long telegramId, string? name)
{
    var response = await Client!.GetAsync($"https://localhost:8081/api/Cache/{telegramId}_current_user_id");
    var userId = await response.Content.ReadAsStringAsync();
    var virtualMachinesResponse = await Client!.GetAsync($"https://localhost:8081/api/VirtualMachine/{userId}/{name}");

    return JsonConvert.DeserializeObject<VirtualMachine>(await virtualMachinesResponse.Content.ReadAsStringAsync());
}
```

Але навіщо кешувати віртуальну машину? І чому цілий об'єкт, а не тільки ідентифікатор? Все просто - кешується об'єкт, бо потім його дані будуть необхідні підчас підключення по SSH, а кешується цілий об'єкт, бо сам він не дуже великий і це не займає багато часу його закешувати та розкешувати, також це швидше, аніж діставати його знову із бази даних по закешованому ідентифікатору
## Створити сервіс для SSH підключення до віртуальної машини
Для підключення до віртуальних машин за допомогою SSH використовується бібліотека C# під назвою Renci.SshNet. Вона надає змогу легко та зручно під'єднатися. Для неї був написаний клас обгортка SSHRequestService, який наслідує інтерфейс ISSHRequestService:
```CSharp
 public interface ISSHRequestService
 {
     Task<string> ExecuteCommandAsync(VirtualMachine virtualMachine, string command, CommandType type);
 }

 public enum CommandType
 {
     NotSudo,
     Sudo
 }
```
```CSharp
public class SSHRequestService : ISSHRequestService
{
    public SshClient? Client { get; private set; }

    public Task<string> ExecuteCommandAsync(VirtualMachine virtualMachine, string command, CommandType type)
    {
        var method = new PasswordAuthenticationMethod(virtualMachine.UserName, virtualMachine.Password);
        var connection = new ConnectionInfo(virtualMachine.Host, virtualMachine.Port, virtualMachine.UserName, method);

        using (Client = new SshClient(connection))
        {
            switch (type)
            {
                case CommandType.NotSudo:
                    {
                        return Task.FromResult(string.Empty);
                    }
                case CommandType.Sudo:
                    {
                        return Task.FromResult(string.Empty);
                    }
                default:
                    {
                        return Task.FromResult(string.Empty);
                    }
            }
        }
    }
}
```

Як видно із коду вище метод ExecuteCommandAsync приймає параметрами об'єкт віртуальної машини, команду та тип команди, який може бути "sudo" або "не sudo". Із об'єкт віртуальної машини цей метод отримує всю необхідну інформацію для під'єднання, а потім  базуючись на типі команди виконує її
## Додати в сервіс метод для виконання "не sudo" команд
Метод, який відповідає за виконання "не sudo" команд, під'єднується до віртуальної машини, а потім виконує команду та повертає її результат:
```CSharp
private async Task<string> ExecuteNotSudoCommandAsync(string command)
{
    await Client!.ConnectAsync(CancellationTokenSource.Token);

    return Client.RunCommand(command).Result;
}
```