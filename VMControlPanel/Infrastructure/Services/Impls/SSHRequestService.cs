using Core.Entities;
using Infrastructure.Services.Interfaces;
using Renci.SshNet;
using Renci.SshNet.Common;
using System.Text.RegularExpressions;

namespace Infrastructure.Services.Impls
{
    public class SSHRequestService : ISSHRequestService
    {
        public SshClient? Client { get; private set; }
        public CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        public async Task<string> ExecuteCommandAsync(VirtualMachine virtualMachine, string command, CommandType type)
        {
            var method = new PasswordAuthenticationMethod(virtualMachine.UserName, virtualMachine.Password);
            var connection = new ConnectionInfo(virtualMachine.Host, virtualMachine.Port, virtualMachine.UserName, method);

            using (Client = new SshClient(connection))
            {
                switch (type)
                {
                    case CommandType.NotSudo:
                        {
                            return await ExecuteNotSudoCommandAsync(command);
                        }
                    case CommandType.Sudo:
                        {
                            return await ExecuteSudoCommandAsync(command, virtualMachine.Password!);
                        }
                    default:
                        {
                            return "Невірний тип команди";
                        }
                }
            }
        }

        private async Task<string> ExecuteNotSudoCommandAsync(string command)
        {
            await Client!.ConnectAsync(CancellationTokenSource.Token);

            return Client.RunCommand(command).Result;
        }

        private async Task<string> ExecuteSudoCommandAsync(string command, string password)
        {
            await Client!.ConnectAsync(CancellationTokenSource.Token);

            IDictionary<TerminalModes, uint> modes = new Dictionary<TerminalModes, uint>
            {
                { TerminalModes.ECHO, 53 }
            };
            ShellStream shellStream = Client.CreateShellStream("xterm", 80, 24, 800, 600, 1024, modes);
            var outputString = string.Empty;

            outputString += $"\n{shellStream.Expect(new Regex(@"[$>]"))}";
            shellStream.WriteLine(command);
            outputString += $"\n{shellStream.Expect(new Regex(@"([$#>:])"))}";
            shellStream.WriteLine(password);
            outputString += $"\n{shellStream.Expect(new Regex(@"[$>]"))}";

            return outputString;
        }
    }
}
