using Core.Entities;
using Infrastructure.Services.Interfaces;
using Renci.SshNet;

namespace Infrastructure.Services.Impls
{
    public class SFTPRequestService : ISFTPRequestService
    {
        private Dictionary<string, SftpClient> _clients = new Dictionary<string, SftpClient>();

        private SftpClient GetClient (VirtualMachine virtualMachine, string userId)
        {
            if (_clients.TryGetValue(userId, out SftpClient? client))
            {
                return client;
            }
            else
            {
                var method = new PasswordAuthenticationMethod(virtualMachine.UserName, virtualMachine.Password);
                var connection = new ConnectionInfo(virtualMachine.Host, virtualMachine.Port, virtualMachine.UserName, method);
                
                client = new SftpClient(connection);
                _clients.Add(userId, client);

                return client;
            }
        }
    }
}
