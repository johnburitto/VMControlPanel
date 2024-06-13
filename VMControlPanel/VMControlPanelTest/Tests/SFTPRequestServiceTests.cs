using Core.Dtos;
using Infrastructure.Services.Impls;
using Infrastructure.Services.Interfaces;
using Utilities;

namespace VMControlPanelTest.NotGitTests
{
    public class SFTPRequestServiceTests
    {
        private readonly ISFTPRequestService _underTest;
        private readonly ISSHRequestService _sshService;

        public SFTPRequestServiceTests()
        {
            _underTest = new SFTPRequestService();
            _sshService = new SSHRequestService(_underTest);
        }

        [Fact]
        public async Task CreateDeleteDirectoryAsync_NormalFlow()
        {
            // Given
            var dto = new SFTPRequestDto
            {
                VirtualMachine = new()
                {
                    Name = "LocalMachine",
                    UserName = "johnburitto",
                    PasswordEncrypted = CryptoService.Blowfish("10utezez", true),
                    Host = "127.0.0.1",
                    Port = 22,
                    UserId = "1"
                },
                Data = "test",
                UserId = "1",
                TelegramId = 1
            };

            // When
            var result = await _underTest.CreateDirectoryAsync(dto);
            var chech = await _sshService.ExecuteCommandAsync(dto.VirtualMachine, "ls", dto.UserId);

            // Then
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal($"Directory successfully created\nTo go in it write command \"cd {dto.Data}\"", result);
            Assert.NotNull(chech);
            Assert.NotEmpty(chech);
            Assert.Contains(dto.Data, chech);

            await DeleteDirectoryAsync_NormalFlow();
        }

        [Fact]
        public async Task CreateDirectoryAsync_Error()
        {
            // Given
            var dto = new SFTPRequestDto
            {
                VirtualMachine = new()
                {
                    Name = "LocalMachine",
                    UserName = "johnburitto",
                    PasswordEncrypted = CryptoService.Blowfish("10utezez", true),
                    Host = "127.0.0.1",
                    Port = 23,
                    UserId = "2"
                },
                Data = "test",
                UserId = "2",
                TelegramId = 1
            };

            // When
            var result = await _underTest.CreateDirectoryAsync(dto);

            // Then
            Assert.Contains($"Some error has occurred during creating:", result);
        }

        private async Task DeleteDirectoryAsync_NormalFlow()
        {
            // Given
            var dto = new SFTPRequestDto
            {
                VirtualMachine = new()
                {
                    Name = "LocalMachine",
                    UserName = "johnburitto",
                    PasswordEncrypted = CryptoService.Blowfish("10utezez", true),
                    Host = "127.0.0.1",
                    Port = 22,
                    UserId = "1"
                },
                Data = "test",
                UserId = "1",
                TelegramId = 1
            };

            // When
            var result = await _underTest.DeleteDirectoryAsync(dto);
            var chech = await _sshService.ExecuteCommandAsync(dto.VirtualMachine, "ls", dto.UserId);

            // Then
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal($"Directory successfully deleted", result);
            Assert.NotNull(chech);
            Assert.NotEmpty(chech);
            Assert.DoesNotContain(dto.Data, chech);
        }

        [Fact]
        public async Task DeleteDirectoryAsync_Error()
        {
            // Given
            var dto = new SFTPRequestDto
            {
                VirtualMachine = new()
                {
                    Name = "LocalMachine",
                    UserName = "johnburitto",
                    PasswordEncrypted = CryptoService.Blowfish("10utezez", true),
                    Host = "127.0.0.1",
                    Port = 23,
                    UserId = "2"
                },
                Data = "test",
                UserId = "2",
                TelegramId = 1
            };

            // When
            var result = await _underTest.CreateDirectoryAsync(dto);

            // Then
            Assert.Contains($"Some error has occurred during creating:", result);
        }

        [Fact]
        public async Task GetFileAsync_NormalFlow()
        {
            // Given
            var dto = new SFTPRequestDto
            {
                VirtualMachine = new()
                {
                    Name = "LocalMachine",
                    UserName = "johnburitto",
                    PasswordEncrypted = CryptoService.Blowfish("10utezez", true),
                    Host = "127.0.0.1",
                    Port = 22,
                    UserId = "2"
                },
                Data = "file.txt",
                UserId = "2",
                TelegramId = 1
            };

            // When
            var result = await _underTest.GetFileAsync(dto);

            // Then
            Assert.NotNull(result);
            Assert.Contains(FileManager.FileDirectory, result.FilePath);
            Assert.Contains(dto.Data, result.FilePath);
            Assert.NotNull(result.Message);
            Assert.NotEmpty(result.Message);
            Assert.Equal($"File {dto.Data} successfully downloaded", result.Message);
        }

        [Fact]
        public async Task GetFileAsync_Error()
        {
            // Given
            var dto = new SFTPRequestDto
            {
                VirtualMachine = new()
                {
                    Name = "LocalMachine",
                    UserName = "johnburitto",
                    PasswordEncrypted = CryptoService.Blowfish("10utezez", true),
                    Host = "127.0.0.1",
                    Port = 23,
                    UserId = "2"
                },
                FilePath = $"test.txt",
                Data = "test",
                UserId = "2",
                TelegramId = 1
            };

            // When
            var result = await _underTest.GetFileAsync(dto);

            // Then
            Assert.NotNull(result);
            Assert.NotNull(result.Message);
            Assert.NotEmpty(result.Message);
            Assert.Contains($"Some error has occurred during downloading file:", result.Message);
        }

        [Fact]
        public async Task UploadFileAsync_NormalFlow()
        {
            // Given
            var dto = new SFTPRequestDto
            {
                VirtualMachine = new()
                {
                    Name = "LocalMachine",
                    UserName = "johnburitto",
                    PasswordEncrypted = CryptoService.Blowfish("10utezez", true),
                    Host = "127.0.0.1",
                    Port = 22,
                    UserId = "2"
                },
                FilePath = $"{FileManager.FileDirectory}/file.txt",
                UserId = "2",
                TelegramId = 1
            };

            // When
            var result = await _underTest.UploadFileAsync(dto);

            // Then
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal($"File {dto.Data} successfully uploaded", result);
        }

        [Fact]
        public async Task UploadFileAsync_Error()
        {
            // Given
            var dto = new SFTPRequestDto
            {
                VirtualMachine = new()
                {
                    Name = "LocalMachine",
                    UserName = "johnburitto",
                    PasswordEncrypted = CryptoService.Blowfish("10utezez", true),
                    Host = "127.0.0.1",
                    Port = 23,
                    UserId = "2"
                },
                FilePath = $"{FileManager.FileDirectory}/test.txt",
                Data = "test",
                UserId = "2",
                TelegramId = 1
            };

            // When
            var result = await _underTest.UploadFileAsync(dto);

            // Then
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains($"Some error has occurred during uploading file:", result);
        }
    }
}
