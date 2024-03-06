using Core.Dtos;
using Core.Entities;
using Moq;
using Moq.EntityFrameworkCore;
using UserInfrastructure.Data;
using UserInfrastructure.Service.Imls;
using UserInfrastructure.Service.Interfaces;

namespace VMControlPanelTest.Tests
{
    public class UserServiceTest
    {
        private readonly Mock<UserDbContext> _context = new();
        private readonly UserService _underTest;

        public UserServiceTest()
        {
            _underTest = new UserService(_context.Object);
        }

        [Fact]
        public async Task CheckIfUserHasAccountAsync_NormalFlow()
        {
            // Given
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                TelegramId = 1,
                UserName = "johnburitto",
                Email = "johnburitto@gmail.com"
            };

            _context.Setup(_ => _.Users).ReturnsDbSet([user]);

            // When
            var result = await _underTest.CheckIfUserHasAccountAsync(1);

            // Then
            Assert.True(result);
        }

        [Fact]
        public async Task CheckIfUserHasAccountAsync_UserDoesNotHaveAccounts()
        {
            // Given
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                TelegramId = 1,
                UserName = "johnburitto",
                Email = "johnburitto@gmail.com"
            };

            _context.Setup(_ => _.Users).ReturnsDbSet([user]);

            // When
            var result = await _underTest.CheckIfUserHasAccountAsync(2);

            // Then
            Assert.False(result);
        }

        [Fact]
        public async Task CheckIfAccountWithUserNameExistAsync_NormalFlow()
        {
            // Given
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                TelegramId = 1,
                UserName = "johnburitto",
                Email = "johnburitto@gmail.com"
            };

            _context.Setup(_ => _.Users).ReturnsDbSet([user]);

            // When
            var result = await _underTest.CheckIfAccountWithUserNameExistAsync("johnburitto");

            // Then
            Assert.True(result);
        }

        [Fact]
        public async Task CheckIfAccountWithUserNameExistAsync_AccountWithSuchUserNameDoesNotExist()
        {
            // Given
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                TelegramId = 1,
                UserName = "johnburitto",
                Email = "johnburitto@gmail.com"
            };

            _context.Setup(_ => _.Users).ReturnsDbSet([user]);

            // When
            var result = await _underTest.CheckIfAccountWithUserNameExistAsync("capitan_soap");

            // Then
            Assert.False(result);
        }

        [Fact]
        public async Task LoginAsync_NormalFlow()
        {
            // Given
            var dto = new LoginDto
            {
                UserName = "johnburitto",
                Password = "214365879"
            };
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                TelegramId = 1,
                UserName = "johnburitto",
                PasswordHash = "3b28b8bcf3237db3512678e0dd506b9515643e2e0af1aa13c0a6779ab676d85a",
                Email = "johnburitto@gmail.com"
            };

            _context.Setup(_ => _.Users).ReturnsDbSet([user]);

            // When
            var result = await _underTest.LoginAsync(dto);

            // Then
            Assert.Equal(AuthResponse.SuccessesLogin, result);
        }

        [Fact]
        public async Task LoginAsync_BadUserName()
        {
            // Given
            var dto = new LoginDto
            {
                UserName = "johnbur1tto",
                Password = "214365879"
            };
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                TelegramId = 1,
                UserName = "johnburitto",
                PasswordHash = "3b28b8bcf3237db3512678e0dd506b9515643e2e0af1aa13c0a6779ab676d85a",
                Email = "johnburitto@gmail.com"
            };

            _context.Setup(_ => _.Users).ReturnsDbSet([user]);

            // When
            var result = await _underTest.LoginAsync(dto);

            // Then
            Assert.Equal(AuthResponse.BadCredentials, result);
        }

        [Fact]
        public async Task LoginAsync_BadPassword()
        {
            // Given
            var dto = new LoginDto
            {
                UserName = "johnburitto",
                Password = "2143658790"
            };
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                TelegramId = 1,
                UserName = "johnburitto",
                PasswordHash = "3b28b8bcf3237db3512678e0dd506b9515643e2e0af1aa13c0a6779ab676d85a",
                Email = "johnburitto@gmail.com"
            };

            _context.Setup(_ => _.Users).ReturnsDbSet([user]);

            // When
            var result = await _underTest.LoginAsync(dto);

            // Then
            Assert.Equal(AuthResponse.BadCredentials, result);
        }

        [Fact]
        public async Task RegisterAsync_NormalFlow()
        {
            // Given
            var dto = new RegisterDto
            {
                TelegramId = 12324567890,
                UserName = "johnburitto",
                Password = "214365879",
                Email = "johnburitto@gmail.com"
            };

            _context.Setup(_ => _.Users).ReturnsDbSet([]);

            // Then
            var result = await _underTest.RegisterAsync(dto);

            // When
            Assert.Equal(AuthResponse.SuccessesRegister, result);
            _context.Verify(_ => _.Users.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once());
            _context.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task RegisterAsync_AccountWithSuchUserNameAlreadyExist()
        {
            // Given
            var dto = new RegisterDto
            {
                TelegramId = 12324567890,
                UserName = "johnburitto",
                Password = "214365879",
                Email = "johnburitto@gmail.com"
            };
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                TelegramId = 1,
                UserName = "johnburitto",
                PasswordHash = "3b28b8bcf3237db3512678e0dd506b9515643e2e0af1aa13c0a6779ab676d85a",
                Email = "johnburitto@gmail.com"
            };

            _context.Setup(_ => _.Users).ReturnsDbSet([user]);

            // Then
            var result = await _underTest.RegisterAsync(dto);

            // When
            Assert.Equal(AuthResponse.AlreadyRegistered, result);
            _context.Verify(_ => _.Users.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never());
            _context.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }
        
        [Fact]
        public async Task RegisterAndLoginAsync_AccountWithSuchUserNameAlreadyExist()
        {
            // Given
            var dto = new RegisterDto
            {
                TelegramId = 12324567890,
                UserName = "johnburitto",
                Password = "214365879",
                Email = "johnburitto@gmail.com"
            };
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                TelegramId = 1,
                UserName = "johnburitto",
                PasswordHash = "3b28b8bcf3237db3512678e0dd506b9515643e2e0af1aa13c0a6779ab676d85a",
                Email = "johnburitto@gmail.com"
            };

            _context.Setup(_ => _.Users).ReturnsDbSet([user]);

            // Then
            var result = await _underTest.RegisterAsync(dto);

            // When
            Assert.Equal(AuthResponse.AlreadyRegistered, result);
            _context.Verify(_ => _.Users.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never());
            _context.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }
    }
}
