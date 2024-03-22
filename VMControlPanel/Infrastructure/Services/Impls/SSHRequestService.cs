using Core.Entities;
using Infrastructure.Services.Interfaces;
using Renci.SshNet;

namespace Infrastructure.Services.Impls
{
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
}
