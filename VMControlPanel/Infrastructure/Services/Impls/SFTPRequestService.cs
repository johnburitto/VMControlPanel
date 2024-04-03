using Core.Dtos;
using Core.Entities;
using Infrastructure.Services.Interfaces;
using Renci.SshNet;

namespace Infrastructure.Services.Impls
{
    public class SFTPRequestService : ISFTPRequestService
    {
        private Dictionary<string, SftpClient> _clients = new Dictionary<string, SftpClient>();
        private const string FILE_DIRECTORY = "Files";

        public CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        public async Task<string> CreateDirectoryAsync(SFTPRequestDto dto)
        {
            var client = GetClient(dto.VirtualMachine!, dto.UserId!);

            try
            {
                await client.ConnectAsync(CancellationTokenSource.Token);
                client.CreateDirectory(dto.Data);

                return $"Directory successfully created\nTo go in it write command \"cd {dto.Data}\"";
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

                return $"Directory successfully deleted";
            }
            catch (Exception e)
            {
                return $"Some error has occurred during deleting:\n{e.Message}";
            }
            finally
            {
                client.Disconnect();
            }
        }

        public async Task<FileDto> GetFileAsync(SFTPRequestDto dto)
        {
            var client = GetClient(dto.VirtualMachine!, dto.UserId!);

            try
            {
                using (var stream = File.Create($"{FILE_DIRECTORY}/{dto.Data!}"))
                {
                    FileManager.CreateDirectory(FILE_DIRECTORY);
                    await client.ConnectAsync(CancellationTokenSource.Token);
                    client.DownloadFile(dto.Data, stream);
                }

                return new()
                {
                    FilePath = $"{FILE_DIRECTORY}/{dto.Data!}",
                    Message = $"File {dto.Data} successfully downloaded",
                    IsUploaded = true
                };
            }
            catch (Exception e)
            {
                return new()
                {
                    Message = $"Some error has occurred during downloading file:\n{e.Message}"
                };
            }
            finally
            {
                client.Disconnect();
            }
        }

        public async Task<string> UploadFileAsync(SFTPRequestDto dto)
        {
            return string.Empty;
            //var client = GetClient(dto.VirtualMachine!, dto.UserId!);

            //try
            //{
            //    await client.ConnectAsync(CancellationTokenSource.Token);
            //    client.UploadFile(dto.File, dto.Data, true);

            //    return $"File {dto.Data} successfully uploaded";
            //}
            //catch (Exception e)
            //{
            //    return $"Some error has occurred during uploading file:\n{e.Message}";
            //}
            //finally
            //{
            //    client.Disconnect();
            //}
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
