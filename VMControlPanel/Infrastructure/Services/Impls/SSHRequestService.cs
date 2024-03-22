using Core.Entities;
using Infrastructure.Services.Interfaces;
using Renci.SshNet;

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
                            return string.Empty;
                        }
                    default:
                        {
                            return string.Empty;
                        }
                }
            }
        }

        private async Task<string> ExecuteNotSudoCommandAsync(string command)
        {
            await Client!.ConnectAsync(CancellationTokenSource.Token);

            return Client.RunCommand(command).Result;
        }
    }
}
