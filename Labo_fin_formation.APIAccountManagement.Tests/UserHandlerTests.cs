using System.Threading;
using System.Threading.Tasks;
using Labo_fin_formation.APIAccountManagement.CQRS_Handlers;
using Labo_fin_formation.APIAccountManagement.Domain.Entities;
using Labo_fin_formation.APIAccountManagement.Models.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using static Labo_fin_formation.APIAccountManagement.CQRS_Commands.UserCommands;
using static Labo_fin_formation.APIAccountManagement.CQRS_Queries.GetUserQueries;

namespace Labo_fin_formation.APIAccountManagement.Tests
{
    public class UserHandlerTests
    {
        [Fact]
        public async Task RegisterUserHandler_ShouldReturnUserName_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();

            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                    userStoreMock.Object,
                    new Mock<IOptions<IdentityOptions>>().Object,
                    new Mock<IPasswordHasher<ApplicationUser>>().Object,
                    new List<IUserValidator<ApplicationUser>>(),
                    new List<IPasswordValidator<ApplicationUser>>(),
                    new Mock<ILookupNormalizer>().Object,
                    new Mock<IdentityErrorDescriber>().Object,
                    new Mock<IServiceProvider>().Object,
                    new Mock<ILogger<UserManager<ApplicationUser>>>().Object
                );


            userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var handler = new UserHandler.RegisterUserHandler(userManagerMock.Object);

            var request = new RegisterUserCommand(new RegisterDto
            {
                Email = "test@example.com",
                Password = "Password123!",
                FirstName = "John",
                LastName = "Doe",
                GdprAccepted = true // <- ici GDPR doit être accepté pour que ça fonctionne
            });

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal("JohnDoe", result); // ou peut-être "John.Doe" selon ta logique
        }


        [Fact]
        public async Task RegisterUserHandler_ShouldReturnNull_WhenGdprNotAccepted()
        {
            // Arrange
            var store = new Mock<IUserStore<ApplicationUser>>();
            var userManager = new UserManager<ApplicationUser>(
                store.Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<ApplicationUser>>().Object,
                new List<IUserValidator<ApplicationUser>>(),
                new List<IPasswordValidator<ApplicationUser>>(),
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<ApplicationUser>>>().Object
            );

            var handler = new UserHandler.RegisterUserHandler(userManager);

            var request = new RegisterUserCommand(new RegisterDto
            {
                Email = "test@example.com",
                Password = "Password123!",
                FirstName = "John",
                LastName = "Doe",
                GdprAccepted = false // important
            });

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }


        [Fact]
        public async Task LoginUserHandler_ShouldReturnUser_WhenLoginIsSuccessful()
        {
            // Arrange
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();

            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                    userStoreMock.Object,
                    new Mock<IOptions<IdentityOptions>>().Object,
                    new Mock<IPasswordHasher<ApplicationUser>>().Object,
                    new List<IUserValidator<ApplicationUser>>(),
                    new List<IPasswordValidator<ApplicationUser>>(),
                    new Mock<ILookupNormalizer>().Object,
                    new Mock<IdentityErrorDescriber>().Object,
                    new Mock<IServiceProvider>().Object,
                    new Mock<ILogger<UserManager<ApplicationUser>>>().Object
                );

            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();

            var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                userManagerMock.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<ILogger<SignInManager<ApplicationUser>>>().Object,
                new Mock<IAuthenticationSchemeProvider>().Object,
                new Mock<IUserConfirmation<ApplicationUser>>().Object
            );

            var user = new ApplicationUser { Email = "test@example.com" };
            userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            userManagerMock.Setup(um => um.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var handler = new UserHandler.LoginUserHandler(signInManagerMock.Object, userManagerMock.Object);
            var request = new LoginUserCommand(new LoginDto
            {
                Email = "test@example.com",
                Password = "Password123!"
            });

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(user, result);
        }

        [Fact]
        public async Task LoginUserHandler_ShouldReturnNull_WhenUserNotFound()
        {
            // Arrange
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();

            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                    userStoreMock.Object,
                    new Mock<IOptions<IdentityOptions>>().Object,
                    new Mock<IPasswordHasher<ApplicationUser>>().Object,
                    new List<IUserValidator<ApplicationUser>>(),
                    new List<IPasswordValidator<ApplicationUser>>(),
                    new Mock<ILookupNormalizer>().Object,
                    new Mock<IdentityErrorDescriber>().Object,
                    new Mock<IServiceProvider>().Object,
                    new Mock<ILogger<UserManager<ApplicationUser>>>().Object
                );


            userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser)null);
            
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();

            var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                userManagerMock.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<ILogger<SignInManager<ApplicationUser>>>().Object,
                new Mock<IAuthenticationSchemeProvider>().Object,
                new Mock<IUserConfirmation<ApplicationUser>>().Object
            );


            var handler = new UserHandler.LoginUserHandler(signInManagerMock.Object, userManagerMock.Object);
            var request = new LoginUserCommand(new LoginDto
            {
                Email = "test@example.com",
                Password = "Password123!"
            });

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LogoutUserHandler_ShouldSignOutUser()
        {
            // Arrange
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<ApplicationUser>>().Object,
                new List<IUserValidator<ApplicationUser>>(),
                new List<IPasswordValidator<ApplicationUser>>(),
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<ApplicationUser>>>().Object
            );

            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();

            var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                userManagerMock.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<ILogger<SignInManager<ApplicationUser>>>().Object,
                new Mock<IAuthenticationSchemeProvider>().Object,
                new Mock<IUserConfirmation<ApplicationUser>>().Object
            );

            signInManagerMock.Setup(sm => sm.SignOutAsync()).Returns(Task.CompletedTask);

            var handler = new UserHandler.LogoutUserHandler(signInManagerMock.Object);
            var request = new LogoutUserCommand("test@example.com");

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert
            signInManagerMock.Verify(sm => sm.SignOutAsync(), Times.Once);
        }

        [Fact]
        public async Task GetUserByIdHandler_ShouldReturnUserDto_WhenUserExists()
        {
            // Arrange
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();

            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                    userStoreMock.Object,
                    new Mock<IOptions<IdentityOptions>>().Object,
                    new Mock<IPasswordHasher<ApplicationUser>>().Object,
                    new List<IUserValidator<ApplicationUser>>(),
                    new List<IPasswordValidator<ApplicationUser>>(),
                    new Mock<ILookupNormalizer>().Object,
                    new Mock<IdentityErrorDescriber>().Object,
                    new Mock<IServiceProvider>().Object,
                    new Mock<ILogger<UserManager<ApplicationUser>>>().Object
                );
            var user = new ApplicationUser { Id = "1", UserName = "testuser" };
            var roles = new List<string> { "User" };

            userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            userManagerMock.Setup(um => um.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(roles);

            var handler = new UserHandler.GetUserByIdHandler(userManagerMock.Object);
            var request = new GetUserByIDQuery("1");

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.UserName, result.UserName);
            Assert.Equal(roles, result.Roles);
        }

        [Fact]
        public async Task GetUserByIdHandler_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();

            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                    userStoreMock.Object,
                    new Mock<IOptions<IdentityOptions>>().Object,
                    new Mock<IPasswordHasher<ApplicationUser>>().Object,
                    new List<IUserValidator<ApplicationUser>>(),
                    new List<IPasswordValidator<ApplicationUser>>(),
                    new Mock<ILookupNormalizer>().Object,
                    new Mock<IdentityErrorDescriber>().Object,
                    new Mock<IServiceProvider>().Object,
                    new Mock<ILogger<UserManager<ApplicationUser>>>().Object
                );
            userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser)null);

            var handler = new UserHandler.GetUserByIdHandler(userManagerMock.Object);
            var request = new GetUserByIDQuery("1");

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }
    }
}
