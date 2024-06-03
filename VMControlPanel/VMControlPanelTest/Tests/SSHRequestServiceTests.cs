using Core.Dtos;
using Core.Entities;
using Infrastructure.Services.Impls;
using Infrastructure.Services.Interfaces;
using System.Net.Sockets;
using Utilities;

namespace VMControlPanelTest.Tests
{
    public class SSHRequestServiceTests
    {
        private readonly ISSHRequestService _underTest;

        public SSHRequestServiceTests()
        {
            _underTest = new SSHRequestService();
        }

        [Fact]
        public async Task ExecuteCommandAsync_NormalFlow()
        {
            // Given
            var dto = new SSHRequestDto
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
                Command = "pwd",
                UserId = "1",
                TelegramId = 1
            };

            // When
            var result = await _underTest.ExecuteCommandAsync(dto.VirtualMachine, dto.Command, dto.UserId);

            // Then
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("/home/johnburitto", result);
        }
        
        [Fact]
        public async Task ExecuteCommandAsync_BadCommand()
        {
            // Given
            var dto = new SSHRequestDto
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
                Command = "badcommand",
                UserId = "1",
                TelegramId = 1
            };

            // When
            var result = await _underTest.ExecuteCommandAsync(dto.VirtualMachine, dto.Command, dto.UserId);

            // Then
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("command not found", result);
        }

        [Fact]
        public async Task ExecuteCommandAsync_BadConnectionInfo()
        {
            // Given
            var dto = new SSHRequestDto
            {
                VirtualMachine = new()
                {
                    Name = "LocalMachine",
                    UserName = "johnburitto",
                    PasswordEncrypted = CryptoService.Blowfish("10utezez", true),
                    Host = "127.0.0.10",
                    Port = 23,
                    UserId = "1"
                },
                Command = "pwd",
                UserId = "1",
                TelegramId = 1
            };

            // When
            var result = await Assert.ThrowsAsync<SocketException>(async () => await _underTest.ExecuteCommandAsync(dto.VirtualMachine, dto.Command, dto.UserId));

            // Then
            Assert.Equal("No connection could be made because the target machine actively refused it.", result.Message);
        }
        
        [Fact]
        public async Task ExecuteCommandAsync_NoUserId()
        {
            // Given
            var dto = new SSHRequestDto
            {
                VirtualMachine = new()
                {
                    Name = "LocalMachine",
                    UserName = "johnburitto",
                    PasswordEncrypted = CryptoService.Blowfish("10utezez", true),
                    Host = "127.0.0.1",
                    Port = 23,
                    UserId = "1"
                },
                Command = "pwd",
                UserId = "1",
                TelegramId = 1
            };

            // When
            var result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await _underTest.ExecuteCommandAsync(dto.VirtualMachine, dto.Command, null));

            // Then
            Assert.Equal("Value cannot be null. (Parameter 'key')", result.Message);
        }

        [Fact]
        public async Task GetMetricsAsync_NormalFlow()
        {
            // Given
            var virtualMachine = new VirtualMachine()
            {
                Name = "LocalMachine",
                UserName = "johnburitto",
                PasswordEncrypted = CryptoService.Blowfish("10utezez", true),
                Host = "127.0.0.1",
                Port = 22,
                UserId = "1"
            };

            // When
            var result = await _underTest.GetMetricsAsync(virtualMachine, "1");

            // Then
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
    }
}
