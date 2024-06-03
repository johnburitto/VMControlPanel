using AutoMapper;
using Core.Dtos;
using Core.Entities;
using Infrastructure.Data;
using Infrastructure.Services.Impls;
using Infrastructure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Utilities;
using VMControlPanelTest.Profiles;

namespace VMControlPanelTest.Tests
{
    public class VirtualMachineServiceTests
    {
        private readonly Mock<AppDbContext> _contex = new();
        private readonly IMapper _mapper;
        private readonly IVirtualMachineService _underTest;

        public VirtualMachineServiceTests()
        {
            _mapper = new Mapper(new MapperConfiguration(configuration => configuration.AddProfile(new VirtualMachineProfile())));
            _underTest = new VirtualMachineService(_contex.Object, _mapper);
        }

        [Fact]
        public async Task CreateAsync_NormalFlow()
        {
            // Given
            var dto = new VirtualMachineDto
            {
                Name = "LocalMachine",
                UserName = "johnburitto",
                Password = "10utezez",
                Host = "127.0.0.1",
                Port = 22,
                UserId = "1",
                TelegramId = 1
            };
            var encryptedPassword = CryptoService.Blowfish("10utezez", true);

            _contex.Setup(_ => _.VirtualMachines).ReturnsDbSet([]);

            // When
            var result = await _underTest.CreateAsync(dto);

            // Then
            Assert.NotNull(result);
            Assert.IsType<VirtualMachine>(result);
            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(dto.UserName, result.UserName);
            Assert.Equal(encryptedPassword, result.PasswordEncrypted);
            Assert.Equal(dto.Host, result.Host);
            Assert.Equal(dto.Port, result.Port);
            Assert.Equal(dto.UserId, result.UserId);
            _contex.Verify(_ => _.VirtualMachines.AddAsync(It.IsAny<VirtualMachine>(), It.IsAny<CancellationToken>()), Times.Once);
            _contex.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public async Task CreateAsync_VirtualMachineWithThisNameAlreadyAdded()
        {
            // Given
            var dto = new VirtualMachineDto
            {
                Name = "LocalMachine",
                UserName = "johnburitto",
                Password = "10utezez",
                Host = "127.0.0.1",
                Port = 22,
                UserId = "1",
                TelegramId = 1
            };
            var virtualMachine = new VirtualMachine
            {
                Name = "LocalMachine",
                UserName = "johnburitto",
                PasswordEncrypted = CryptoService.Blowfish("10utezez", true),
                Host = "127.0.0.1",
                Port = 22,
                UserId = "1",
            };

            _contex.Setup(_ => _.VirtualMachines).ReturnsDbSet([virtualMachine]);

            // When
            var result = await Assert.ThrowsAsync<Exception>(async () => await _underTest.CreateAsync(dto));

            // Then
            Assert.Equal($"Virtual machine with name {dto.Name} already added", result.Message);
        }

        [Fact]
        public async Task DeleteAsync_NormalFlow()
        {
            // Given
            var virtualMachine = new VirtualMachine
            {
                Id = 1,
                Name = "LocalMachine",
                UserName = "johnburitto",
                PasswordEncrypted = CryptoService.Blowfish("10utezez", true),
                Host = "127.0.0.1",
                Port = 22,
                UserId = "1",
            };

            _contex.Setup(_ => _.VirtualMachines).ReturnsDbSet([virtualMachine]);

            // When
            await _underTest.DeleteAsync(virtualMachine);

            // Then
            _contex.Verify(_ => _.VirtualMachines.Remove(It.IsAny<VirtualMachine>()), Times.Once);
            _contex.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_NormalFlow()
        {
            // Given
            var virtualMachine = new VirtualMachine
            {
                Id = 1,
                Name = "LocalMachine",
                UserName = "johnburitto",
                PasswordEncrypted = CryptoService.Blowfish("10utezez", true),
                Host = "127.0.0.1",
                Port = 22,
                UserId = "1",
            };

            _contex.Setup(_ => _.VirtualMachines).ReturnsDbSet([virtualMachine]);

            // When
            var result = await _underTest.GetAllAsync();

            // Then
            Assert.NotNull(result);
            Assert.IsType<List<VirtualMachine>>(result);
            Assert.Single(result);
            Assert.Equal([virtualMachine], result);
        }

        [Fact]
        public async Task GetAllAsync_EmptyDatabase()
        {
            // Given
            _contex.Setup(_ => _.VirtualMachines).ReturnsDbSet([]);

            // When
            var result = await _underTest.GetAllAsync();

            // Then
            Assert.NotNull(result);
            Assert.IsType<List<VirtualMachine>>(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetByIdAsync_NormalFlow()
        {
            // Given
            var virtualMachine = new VirtualMachine
            {
                Id = 1,
                Name = "LocalMachine",
                UserName = "johnburitto",
                PasswordEncrypted = CryptoService.Blowfish("10utezez", true),
                Host = "127.0.0.1",
                Port = 22,
                UserId = "1",
            };

            _contex.Setup(_ => _.VirtualMachines).ReturnsDbSet([virtualMachine]);

            // When
            var result = await _underTest.GetByIdAsync(1);

            // Then
            Assert.NotNull(result);
            Assert.IsType<VirtualMachine>(result);
            Assert.Equal(virtualMachine.Name, result.Name);
            Assert.Equal(virtualMachine.UserName, result.UserName);
            Assert.Equal(virtualMachine.PasswordEncrypted, result.PasswordEncrypted);
            Assert.Equal(virtualMachine.Password, result.Password);
            Assert.Equal(virtualMachine.Host, result.Host);
            Assert.Equal(virtualMachine.Port, result.Port);
            Assert.Equal(virtualMachine.UserId, result.UserId);
        }
        
        [Fact]
        public async Task GetByIdAsync_TryGetObjectWithIdThatDoesNotExist()
        {
            // Given
            var virtualMachine = new VirtualMachine
            {
                Id = 1,
                Name = "LocalMachine",
                UserName = "johnburitto",
                PasswordEncrypted = CryptoService.Blowfish("10utezez", true),
                Host = "127.0.0.1",
                Port = 22,
                UserId = "1",
            };

            _contex.Setup(_ => _.VirtualMachines).ReturnsDbSet([virtualMachine]);

            // When
            var result = await _underTest.GetByIdAsync(2);

            // Then
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserVirtualMachinesAsync_NormalFlow()
        {
            // Given
            var userVirtualMachine = new VirtualMachine
            {
                Id = 1,
                Name = "LocalMachine",
                UserName = "johnburitto",
                PasswordEncrypted = CryptoService.Blowfish("10utezez", true),
                Host = "127.0.0.1",
                Port = 22,
                UserId = "1",
            };
            var otherUserVirtualMachine = new VirtualMachine
            {
                Id = 1,
                Name = "LocalMachine",
                UserName = "johnburitto",
                PasswordEncrypted = CryptoService.Blowfish("10utezez", true),
                Host = "127.0.0.1",
                Port = 22,
                UserId = "2",
            };

            _contex.Setup(_ => _.VirtualMachines).ReturnsDbSet([userVirtualMachine, otherUserVirtualMachine]);

            // When
            var result = await _underTest.GetUserVirtualMachinesAsync("1");

            // Then
            Assert.NotNull(result);
            Assert.IsType<List<VirtualMachine>>(result);
            Assert.Single(result);

            var virtualMachine = result.First();

            Assert.Equal(userVirtualMachine.Name, virtualMachine.Name);
            Assert.Equal(userVirtualMachine.UserName, virtualMachine.UserName);
            Assert.Equal(userVirtualMachine.PasswordEncrypted, virtualMachine.PasswordEncrypted);
            Assert.Equal(userVirtualMachine.Password, virtualMachine.Password);
            Assert.Equal(userVirtualMachine.Host, virtualMachine.Host);
            Assert.Equal(userVirtualMachine.Port, virtualMachine.Port);
            Assert.Equal(userVirtualMachine.UserId, virtualMachine.UserId);
        }
        
        [Fact]
        public async Task GetUserVirtualMachinesAsync_UserDoesNotHaveVirtualMachines()
        {
            // Given
            _contex.Setup(_ => _.VirtualMachines).ReturnsDbSet([]);

            // When
            var result = await _underTest.GetUserVirtualMachinesAsync("1");

            // Then
            Assert.NotNull(result);
            Assert.IsType<List<VirtualMachine>>(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetVirtualMachineByUserIdAndVMNameAsync_NormalFlow()
        {
            // Given
            var userVirtualMachine = new VirtualMachine
            {
                Id = 1,
                Name = "LocalMachine",
                UserName = "johnburitto",
                PasswordEncrypted = CryptoService.Blowfish("10utezez", true),
                Host = "127.0.0.1",
                Port = 22,
                UserId = "1",
            };
            var otherUserVirtualMachine = new VirtualMachine
            {
                Id = 1,
                Name = "LocalMachine",
                UserName = "johnburitto",
                PasswordEncrypted = CryptoService.Blowfish("10utezez", true),
                Host = "127.0.0.1",
                Port = 22,
                UserId = "2",
            };

            _contex.Setup(_ => _.VirtualMachines).ReturnsDbSet([userVirtualMachine, otherUserVirtualMachine]);

            // When
            var result = await _underTest.GetVirtualMachineByUserIdAndVMNameAsync("1", "LocalMachine");

            // Then
            Assert.NotNull(result);
            Assert.IsType<VirtualMachine>(result);
            Assert.Equal(userVirtualMachine.Name, result.Name);
            Assert.Equal(userVirtualMachine.UserName, result.UserName);
            Assert.Equal(userVirtualMachine.PasswordEncrypted, result.PasswordEncrypted);
            Assert.Equal(userVirtualMachine.Password, result.Password);
            Assert.Equal(userVirtualMachine.Host, result.Host);
            Assert.Equal(userVirtualMachine.Port, result.Port);
            Assert.Equal(userVirtualMachine.UserId, result.UserId);
        }

        [Fact]
        public async Task GetVirtualMachineByUserIdAndVMNameAsync_UserDoesNotHaveVirtualMachineWithSuchName()
        {
            // Given
            var virtualMachine = new VirtualMachine
            {
                Id = 1,
                Name = "LocalMachine",
                UserName = "johnburitto",
                PasswordEncrypted = CryptoService.Blowfish("10utezez", true),
                Host = "127.0.0.1",
                Port = 22,
                UserId = "1",
            };

            _contex.Setup(_ => _.VirtualMachines).ReturnsDbSet([virtualMachine]);

            // When
            var result = await _underTest.GetVirtualMachineByUserIdAndVMNameAsync("1", "AWSMachine");

            // Then
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_NormalFlow()
        {
            // Given
            var virtualMachine = new VirtualMachine
            {
                Id = 1,
                Name = "LocalMachine",
                UserName = "johnbur!tto",
                PasswordEncrypted = CryptoService.Blowfish("11utezez", true),
                Host = "127.0.0.10",
                Port = 24,
                UserId = "1",
            };
            var dto = new VirtualMachineDto
            {
                Name = "LocalMachine",
                UserName = "johnburitto",
                Password = "10utezez",
                Host = "127.0.0.1",
                Port = 22,
                UserId = "1",
                TelegramId = 1
            };

            _contex.Setup(_ => _.VirtualMachines).ReturnsDbSet([virtualMachine]);

            // When
            var result = await _underTest.UpdateAsync(dto);

            // Then
            Assert.NotNull(result);
            Assert.IsType<VirtualMachine>(result);
            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(dto.UserName, result.UserName);
            Assert.Equal(CryptoService.Blowfish(dto.Password, true), result.PasswordEncrypted);
            Assert.Equal(dto.Host, result.Host);
            Assert.Equal(dto.Port, result.Port);
            Assert.Equal(dto.UserId, result.UserId);
        }
        
        [Fact]
        public async Task UpdateAsync_VirtualMachineDoesntExist()
        {
            // Given
            var dto = new VirtualMachineDto
            {
                Name = "LocalMachine",
                UserName = "johnburitto",
                Password = "10utezez",
                Host = "127.0.0.1",
                Port = 22,
                UserId = "1",
                TelegramId = 1
            };

            _contex.Setup(_ => _.VirtualMachines).ReturnsDbSet([]);

            // When
            var result = await Assert.ThrowsAsync<Exception>(async () => await _underTest.UpdateAsync(dto));

            // Then
            Assert.Equal($"Virtual machine with name {dto.Name} doesn't exist", result.Message);
        }
    }
}
