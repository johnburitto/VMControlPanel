using Core.Dtos;
using Core.Entities;
using Infrastructure.Services.Interfaces;
using Renci.SshNet;

namespace Infrastructure.Services.Impls
{
    public class SFTPRequestService : ISFTPRequestService
    {
        private Dictionary<string, SftpClient> _clients = new Dictionary<string, SftpClient>();

        public CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        public async Task<string> CreateDirectoryAsync(SFTPRequestDto dto)
        {
            var client = GetClient(dto.VirtualMachine!, dto.UserId!);

            try
            {
                await client.ConnectAsync(CancellationTokenSource.Token);
                client.CreateDirectory(dto.Data);

                return $"Directory successfully created\nTo go in it write command \"cd ${dto.Data}\"";
            }
            catch (Exception e)
            {
                return $"Some error has occurred during creating:\n{e.Message}";
            }
            finally
            {
                client.Disconnect();
            }
        }

        public async Task<string> DeleteDirectoryAsync(SFTPRequestDto dto)
        {
            var client = GetClient(dto.VirtualMachine!, dto.UserId!);

            try
            {
                await client.ConnectAsync(CancellationTokenSource.Token);
                client.DeleteDirectory(dto.Data);

                return $"Directory successfully deleted\nTo go in it write command \"cd ${dto.Data}\"";
            }
            catch (Exception e)
            {
                return $"Some error has occurred during creating:\n{e.Message}";
            }
            finally
            {
                client.Disconnect();
            }
        }

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
