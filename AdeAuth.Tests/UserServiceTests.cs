using AdeAuth.Db;
using AdeAuth.Models;
using AdeAuth.Services;
using AdeAuth.Services.Interfaces;
using Moq;

namespace AdeAuth.Tests
{
    public class UserServiceTests : DbContextTestHelper
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            identityContext = new(dbOptions);
            passwordManager = new Mock<IPasswordManager>();
            userService = new UserService<IdentityContext,ApplicationUser>
                (identityContext,passwordManager.Object);
        }
        [Test]
        public async Task ShouldCreateUsersSuccessfully()
        {

            ApplicationUser user = new() 
            { 
                FirstName = "Adeola",
                LastName="Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey=string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            var response = await userService.CreateUserAsync(user);

            Assert.IsTrue(response);
        }

        [Test]
        public async Task ShouldAuthenticateUserUsingEmailSuccessfully()
        {
            ApplicationUser user = new()
            {
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };
            passwordManager.Setup(s => s.VerifyPassword(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>())).Returns(true);

           _ =  await userService.CreateUserAsync(user);

           var response = await  userService.AuthenticateUsingEmailAsync("adeolaaderibigbe09@gmail.com", "1234567");

            Assert.That(response, Is.Not.Null);
        }

        [Test]
        public async Task ShouldAuthenticateUserUsingUsernameSuccessfully()
        {
            ApplicationUser user = new()
            {
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName="Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890",
            };
            passwordManager.Setup(s => s.VerifyPassword(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            await userService.CreateUserAsync(user);

            var response = await userService.AuthenticateUsingUsernameAsync("Addie", "1234567");

            Assert.That(response, Is.Not.Null);
        }

        [TearDown]
        public void TearDown()
        {
            identityContext.Database.EnsureDeleted();
        }

        private IdentityContext identityContext;

        private IUserService<ApplicationUser> userService;

        private Mock<IPasswordManager> passwordManager;
    }
}