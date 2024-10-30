using AdeAuth.Db;
using AdeAuth.Models;
using AdeAuth.Services;
using AdeAuth.Services.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Tests
{
    public class RoleServiceTests : DbContextTestHelper
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            identityContext = new(dbOptions);
            var passwordManager = new Mock<IPasswordManager>().Object;
            userService = new UserService<IdentityContext, ApplicationUser>
               (identityContext, passwordManager);
            roleService = new RoleService<IdentityContext,ApplicationUser,ApplicationRole>(identityContext);
        }

        [Test]
        public async Task ShouldAddUserRoleSuccessfully()
        {
            ApplicationUser user = new()
            {
                Id = new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"),
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            ApplicationRole role = new() 
            { 
                Id = Guid.NewGuid(),
                Name = "User"
            };


            _ = await userService.CreateUserAsync(user);

            _ = await roleService.CreateRoleAsync(role);

            var response = await roleService.AddUserRoleAsync(user.Id, "User");

            Assert.True(response);
        }

        [Test]
        public async Task ShouldFailToAddUserRoleIfUserDoesNotExist()
        {
            ApplicationRole role = new()
            {
                Id = Guid.NewGuid(),
                Name = "User"
            };

            _ = await roleService.CreateRoleAsync(role);

            var response = await roleService.AddUserRoleAsync(Guid.NewGuid(), "User");

            Assert.False(response);
        }

        [Test]
        public async Task ShouldFailToAddUserRoleIfRoleDoesNotExist()
        {
            ApplicationUser user = new()
            {
                Id = new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"),
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            _ = await userService.CreateUserAsync(user);

            var response = await roleService.AddUserRoleAsync(user.Id, "User");

            Assert.False(response);
        }

        [Test]
        public async Task ShouldAddRoleSuccessfully()
        {
            ApplicationRole role = new()
            {
                Id = Guid.NewGuid(),
                Name = "User"
            };

            var response = await roleService.CreateRoleAsync(role);

            Assert.True(response);
        }

        [Test]
        public async Task ShouldAddRolesSuccessfully()
        {
            List<ApplicationRole> roles = new()
            {  new()
                {  
                    Id = Guid.NewGuid(),
                    Name = "User"
                }

            };

            var response = await roleService.CreateRolesAsync(roles);

            Assert.True(response);
        }

        [Test]
        public async Task ShouldDeleteRoleSuccessfully()
        {
            ApplicationRole role = new()
            {
                Id = Guid.NewGuid(),
                Name = "User"
            };

            _ = await roleService.CreateRoleAsync(role);

            var response = await roleService.DeleteRoleAsync("User");

            Assert.True(response);
        }

        [Test]
        public async Task ShouldFailDeleteRoleSuccessfully()
        {
            var response = await roleService.DeleteRoleAsync("User");

            Assert.False(response);
        }

        [Test]
        public async Task ShouldDeleteRolesSuccessfully()
        {
            ApplicationRole role = new()
            {
                Id = Guid.NewGuid(),
                Name = "User"
            };

            _ = await roleService.CreateRoleAsync(role);

            var response = await roleService.DeleteRolesAsync(new[] { "User" });

            Assert.True(response);
        }

        [Test]
        public async Task ShouldRemoveUserRoleSuccessfully()
        {
            ApplicationRole role = new()
            {
                Id = Guid.NewGuid(),
                Name = "User"
            };
            ApplicationUser user = new()
            {
                Id = new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"),
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            _ = await userService.CreateUserAsync(user);
            _ = await roleService.CreateRoleAsync(role);
            _ = await roleService.AddUserRoleAsync(user.Id, "User");

            var response = await roleService.RemoveUserRoleAsync(user.Id, "User");

            Assert.True(response);
        }

        [Test]
        public async Task ShouldFailToRemoveUserRoleIfUserDoesNotExist()
        {
            ApplicationRole role = new()
            {
                Id = Guid.NewGuid(),
                Name = "User"
            };

            _ = await roleService.CreateRoleAsync(role);

            var response = await roleService.RemoveUserRoleAsync(Guid.NewGuid(), "User");

            Assert.False(response);
        }

        [Test]
        public async Task ShouldFailToRemoveUserRoleIfRoleDoesNotExist()
        {
            ApplicationUser user = new()
            {
                Id = new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"),
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            _ = await userService.CreateUserAsync(user);

            var response = await roleService.RemoveUserRoleAsync(user.Id, "User");

            Assert.False(response);
        }

        [Test]
        public async Task ShouldFailToRemoveUserRoleIfUserRoleDoesNotExist()
        {
            ApplicationRole role = new()
            {
                Id = Guid.NewGuid(),
                Name = "User"
            };
            ApplicationUser user = new()
            {
                Id = new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"),
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            _ = await userService.CreateUserAsync(user);
            _ = await roleService.CreateRoleAsync(role);

            var response = await roleService.RemoveUserRoleAsync(user.Id, "User");

            Assert.False(response);
        }

        [Test]
        public async Task ShouldAddUserRoleByEmailSuccessfully()
        {
            ApplicationUser user = new()
            {
                Id = new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"),
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            ApplicationRole role = new()
            {
                Id = Guid.NewGuid(),
                Name = "User"
            };


            _ = await userService.CreateUserAsync(user);

            _ = await roleService.CreateRoleAsync(role);

            var response = await roleService.AddUserRoleAsync(user.Email, "User");

            Assert.True(response);
        }

        [Test]
        public async Task ShouldFailToAddUserRoleByEmailIfUserDoesNotExist()
        {
            ApplicationRole role = new()
            {
                Id = Guid.NewGuid(),
                Name = "User"
            };

            _ = await roleService.CreateRoleAsync(role);

            var response = await roleService.AddUserRoleAsync("adeolaaderibigbe09@gmail.com", "User");

            Assert.False(response);
        }

        [Test]
        public async Task ShouldFailToAddUserRoleByEmailIfRoleDoesNotExist()
        {
            ApplicationUser user = new()
            {
                Id = new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"),
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            _ = await userService.CreateUserAsync(user);

            var response = await roleService.AddUserRoleAsync(user.Email, "User");

            Assert.False(response);
        }

        [TearDown]
        public void TearDown()
        {
            identityContext.Database.EnsureDeleted();
        }

        private IdentityContext identityContext;
        private IUserService<ApplicationUser> userService;
        private IRoleService<ApplicationRole> roleService;
    }
}
