using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asp_REST.Data;
using Asp_REST.Dto;
using Asp_REST.Entity;
using Asp_REST.Exceptions;
using Asp_REST.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Asp_REST_TEST
{
    public class AuthenticationServiceTest
    {
        private readonly AppDbContext context;
        private readonly AuthService authService;

        public AuthenticationServiceTest()
        {
            // Konfiguracja InMemory Database
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unikalna baza dla każdego testu
                .Options;

            context = new AppDbContext(options);

            context.Users.Add(new User { Id = 1, Email = "existing@test.com", VerificationCode = "123456" });
            context.SaveChanges();

            var mockConfiguration = new Mock<IConfiguration>();
            var emailService = new EmailService(mockConfiguration.Object); 

            authService = new AuthService(context, mockConfiguration.Object, emailService);
        }

        [Fact]
        public void VerifyUser_WhenUserNotFound_ThrowsUserException()
        {
            // Arrange
            var verifyUserDto = new VerifyUserDto { Email = "notfound@test.com", VerificationCode = "123456" };

            // Act & Assert
            var exception = Assert.Throws<UserException>(() => authService.VerifyUser(verifyUserDto));
            Assert.Equal("Brak takiego użytkownika", exception.Message);
        }
        [Fact]
        public void VerifyUser_WhenUserAlreadyEnabled_ThrowsUserException()
        {
            // Arrange
            var user = new User
            {
                Email = "test@test.com",
                Enabled = true
            };
            context.Users.Add(user);
            context.SaveChanges();

            var verifyUserDto = new VerifyUserDto { Email = "test@test.com", VerificationCode = "123456" };

            // Act & Assert
            var exception = Assert.Throws<UserException>(() => authService.VerifyUser(verifyUserDto));
            Assert.Equal("Użytkownik został już zweryfikowany", exception.Message);
        }

        [Fact]
        public void VerifyUser_WhenVerificationCodeExpired_ThrowsUserException()
        {
            // Arrange
            var user = new User
            {
                Email = "test@test.com",
                Enabled = false,
                VerificationCode = "123456",
                VerificationCodeExpiresAt = DateTime.UtcNow.AddDays(-1) // Kod wygasł
            };
            context.Users.Add(user);
            context.SaveChanges();

            var verifyUserDto = new VerifyUserDto { Email = "test@test.com", VerificationCode = "123456" };

            // Act & Assert
            var exception = Assert.Throws<UserException>(() => authService.VerifyUser(verifyUserDto));
            Assert.Equal("Kod weryfikacyjny stracił swoją ważność", exception.Message);
        }

        [Fact]
        public void VerifyUser_WhenVerificationCodeInvalid_ThrowsUserException()
        {
            // Arrange
            var user = new User
            {
                Email = "test@test.com",
                Enabled = false,
                VerificationCode = "different-code",
                VerificationCodeExpiresAt = DateTime.UtcNow.AddDays(1) // Kod jest nadal ważny
            };
            context.Users.Add(user);
            context.SaveChanges();

            var verifyUserDto = new VerifyUserDto { Email = "test@test.com", VerificationCode = "123456" };

            // Act & Assert
            var exception = Assert.Throws<UserException>(() => authService.VerifyUser(verifyUserDto));
            Assert.Equal("Niepoprawny kod weryfikacyjny", exception.Message);
        }

        [Fact]
        public void VerifyUser_WhenVerificationSuccessful_UpdatesUserAndSavesChanges()
        {
            // Arrange
            var verificationCode = "123456";
            var user = new User
            {
                Email = "test@test.com",
                Enabled = false,
                VerificationCode = verificationCode,
                VerificationCodeExpiresAt = DateTime.UtcNow.AddDays(1)
            };
            context.Users.Add(user);
            context.SaveChanges();

            var verifyUserDto = new VerifyUserDto { Email = "test@test.com", VerificationCode = verificationCode };

            // Act
            authService.VerifyUser(verifyUserDto);

            // Assert
            var updatedUser = context.Users.FirstOrDefault(u => u.Email == "test@test.com");
            Assert.NotNull(updatedUser);
            Assert.True(updatedUser.Enabled);
            Assert.Empty(updatedUser.VerificationCode);
            Assert.True(updatedUser.VerificationCodeExpiresAt <= DateTime.UtcNow);
        }
    }
}
