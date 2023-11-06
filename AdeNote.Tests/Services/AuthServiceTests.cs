using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Services;
using AdeNote.Models;
using Moq;
using System.Text;

namespace AdeNote.Tests.Services
{
    [TestFixture]
    public class AuthServiceTests 
    {
        [SetUp]
        public void SetUp()
        {
            authRepository = new Mock<IAuthRepository>();
            blobService = new Mock<IBlobService>();
            authService = new Mock<AuthService>().Object;
            userDetailRepository = new Mock<IUserDetailRepository>();
            smsService = new Mock<ISmsService>();
            authService.authRepository = authRepository.Object;
            authService.blobService = blobService.Object;
            authService.smsService = smsService.Object;
            authService.userDetailRepository = userDetailRepository.Object;
            authService.key = "testKey";
            authService.loginSecret = "testLoginSecret";
        }

        [Test]
        public async Task ShouldSetAuthenticatorSuccessfully()
        {
            //Arrange
            blobService.Setup(s => s.UploadImage(It.IsAny<string>(), It.IsAny<Stream>(),Infrastructure.Utilities.MimeType.png)).ReturnsAsync("test-url");
            authRepository.Setup(s => s.Add(It.IsAny<UserToken>())).ReturnsAsync(true);

            //Act
            var response = await authService.SetAuthenticator(new Guid("f79cd68f-2aa9-4edc-9427-742109626943"), "email");

            //Assert
            Assert.That(response.IsSuccessful, Is.True);
        }

        [Test]
        public async Task ShouldFailToSetAuthenticator()
        {
            //Arrange
            blobService.Setup(s => s.UploadImage(It.IsAny<string>(), It.IsAny<Stream>(),Infrastructure.Utilities.MimeType.png)).ReturnsAsync("test-url");

            //Act
            var response = await authService.SetAuthenticator(new Guid("f79cd68f-2aa9-4edc-9427-742109626943"), "email");

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors[0], Is.EqualTo("failed to set up two factor authentication"));
        }

        
        [TestCase("f79cd68f-2aa9-4edc-9427-742109626943","", "Invalid email")]
        [TestCase("00000000-0000-0000-0000-000000000000", "email", "Invalid user id")]
        public async Task ShouldFailToSetAuthenticatorIfParametersDoNotExist(Guid userId, string email,string error)
        {
            //Act
            var response = await authService.SetAuthenticator(userId,email);

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors[0], Is.EqualTo(error));
        }

        [TestCase("", "otp", "Invalid email")]
        [TestCase("email", "", "Invalid otp")]
        public void ShouldFailToVerifyAuthenticatorOTP(string email,string otp,string error)
        {
            var response = authService.VerifyAuthenticatorOTP(email, otp);

            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors[0], Is.EqualTo(error));
        }

        [Test]
        public void ShouldFailToVerifyAuthenticatorOTP()
        {

            var response = authService.VerifyAuthenticatorOTP("email", "otp");

            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors[0], Is.EqualTo("Invalid otp"));
        }

        [Test]
        public async Task ShouldGetQrCodeSuccessfully()
        {
            authRepository.Setup(s => s.GetAuthenticationType(It.IsAny<Guid>())).ReturnsAsync(new UserToken());

            var response = await authService.GetUserQrCode(new Guid("f79cd68f-2aa9-4edc-9427-742109626943"));

            Assert.That(response.IsSuccessful, Is.True);
        }

        [TestCase("f79cd68f-2aa9-4edc-9427-742109626943", "No two factor enabled")]
        [TestCase("00000000-0000-0000-0000-000000000000", "Invalid user id")]
        public async Task ShouldFailToGetQRCode(Guid userId, string error)
        {
            var response = await authService.GetUserQrCode(userId);

            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors[0], Is.EqualTo(error));
        }


        [Test]
        public async Task ShouldCheckIfAuthenticatorEnabledSuccessfully()
        {
            authRepository.Setup(s => s.GetAuthenticationType(It.IsAny<Guid>())).ReturnsAsync(new UserToken());

            var response = await authService.IsAuthenticatorEnabled(new Guid("f79cd68f-2aa9-4edc-9427-742109626943"));

            Assert.That(response.IsSuccessful, Is.True);
        }

        [TestCase("f79cd68f-2aa9-4edc-9427-742109626943", "No two factor enabled")]
        [TestCase("00000000-0000-0000-0000-000000000000", "Invalid user id")]
        public async Task ShouldFailToCheckIfAuthenticatorEnabled(Guid userId, string error)
        {
            var response = await authService.IsAuthenticatorEnabled(userId);

            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors[0], Is.EqualTo(error));
        }

        [Test]
        public void ShouldGenerateMFATokenSuccessfully()
        {
            var response = authService.GenerateMFAToken(new Guid("f79cd68f-2aa9-4edc-9427-742109626943"), "email", "refreshToken");
            
            Assert.That(response.IsSuccessful, Is.True);
        }

        [TestCase("00000000-0000-0000-0000-000000000000","email","token","Invalid user id")]
        [TestCase("f79cd68f-2aa9-4edc-9427-742109626943", "","token", "Invalid email or refresh token")]
        public void ShouldFailToGenerateMFAToken(Guid userId,string email,string refreshToken,string error)
        {
            var response = authService.GenerateMFAToken(userId,email,refreshToken);
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors[0], Is.EqualTo(error));
        }

        [Test]
        public void ShouldReadDetailsFromToken()
        {
            var key = $"{authService.loginSecret}-email-f79cd68f2aa94edc9427742109626943-refreshToken";

            var encodedToken = Encoding.UTF8.GetBytes(key);

            var token = Convert.ToBase64String(encodedToken);

            var response = authService.ReadDetailsFromToken(token);

            Assert.That(response.IsSuccessful, Is.True);
        }

        [Test]
        public void ShouldFailToReadDetailsFromToken()
        {
            var key = $"login-email-f79cd68f2aa94edc9427742109626943-refreshToken";

            var encodedToken = Encoding.UTF8.GetBytes(key);

            var token = Convert.ToBase64String(encodedToken);

            var response = authService.ReadDetailsFromToken(token);

            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors[0], Is.EqualTo("Invalid token"));
        }

        [Test]

        public void ShouldFailToReadDetailsFromTokenTokenDoesNotExist()
        {

            var response = authService.ReadDetailsFromToken("");

            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors[0], Is.EqualTo("Invalid token"));
        }

        [Test]
        public async Task ShouldCheckIfAuthenticatorEnabledSuccessfullyUsingEmail()
        {
            authRepository.Setup(s => s.GetAuthenticationType(It.IsAny<string>())).ReturnsAsync(new UserToken());

            var response = await authService.IsAuthenticatorEnabled("email");

            Assert.That(response.IsSuccessful, Is.True);
        }

        [TestCase("email", "No two factor enabled")]
        [TestCase("", "Invalid email")]
        public async Task ShouldFailToCheckIfAuthenticatorEnabledUsingEmail(string email, string error)
        {
            var response = await authService.IsAuthenticatorEnabled(email);

            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors[0], Is.EqualTo(error));
        }

        [Test]
        public async Task ShouldRevokeRefreshToken()
        {
            authRepository.Setup(s=>s.GetRefreshTokenByUserId(It.IsAny<Guid>(),It.IsAny<string>())).ReturnsAsync(new TasksLibrary.Models.RefreshToken("refreshToken"
                ,DateTime.UtcNow.AddDays(3),new TasksLibrary.Models.UserId(new Guid("f79cd68f-2aa9-4edc-9427-742109626943"))));

            var response = await authService.RevokeRefreshToken(new Guid("f79cd68f-2aa9-4edc-9427-742109626943"), "refreshToken");

            Assert.That(response.IsSuccessful, Is.True);
        }

        [TestCase("f79cd68f-2aa9-4edc-9427-742109626943","","Invalid refresh token")]
        [TestCase("00000000-0000-0000-0000-000000000000", "token","Invalid user id")]
        public async Task ShouldFailToRevokeRefreshToken(Guid userId, string refreshToken,string error)
        {
            var response = await authService.RevokeRefreshToken(userId,refreshToken);

            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors[0], Is.EqualTo(error));
        }

        [Test]
        public async Task ShouldFailToRevokeRefreshToken()
        {
            var response = await authService.RevokeRefreshToken(new Guid("f79cd68f-2aa9-4edc-9427-742109626943"), "refreshToken");

            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors[0], Is.EqualTo("Invalid token"));
        }

        [Test]
        public async Task ShouldCheckIfTokenHasBeenRevoked()
        {
            authRepository.Setup(s=>s.GetRefreshToken(It.IsAny<string>())).ReturnsAsync(new TasksLibrary.Models.RefreshToken("refreshToken"
                , DateTime.UtcNow.AddDays(3), new TasksLibrary.Models.UserId(new Guid("f79cd68f-2aa9-4edc-9427-742109626943"))));

            var response = await authService.IsTokenRevoked("Token");

            Assert.That(response.IsSuccessful, Is.True);
        }


        [Test]
        public async Task ShouldFailToCheckIfTokenHasBeenRevokedIfTokenIsEmpty()
        {

            var response = await authService.IsTokenRevoked("");

            Assert.That(response.IsSuccessful, Is.False);

            Assert.That(response.Errors[0], Is.EqualTo("Invalid refresh token"));
        }

        [Test]
        public async Task ShouldFailToCheckIfTokenHasBeenRevokedIfTokenDoesNotExist()
        {
            var response = await authService.IsTokenRevoked("Token");

            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors[0], Is.EqualTo("Invalid token"));
        }
        [Test]
        public async Task ShouldDisableUserMFASucessfully()
        {
            authRepository.Setup(s => s.GetAuthenticationType(It.IsAny<Guid>())).ReturnsAsync(new UserToken());
            authRepository.Setup(s => s.Remove(It.IsAny<UserToken>())).ReturnsAsync(true);

            var response = await authService.DisableUserMFA(new Guid("f79cd68f-2aa9-4edc-9427-742109626943"));

            Assert.That(response.IsSuccessful, Is.True);
        }

        [Test]
        public async Task ShouldFailToDisableUserMFAIfUserTokenDoesNotExist()
        {
            var response = await authService.DisableUserMFA(new Guid("f79cd68f-2aa9-4edc-9427-742109626943"));

            Assert.That(response.IsSuccessful, Is.False);
        }

        [Test]
        public async Task ShouldFailToDisableUserMFAIfUserTokenIsFailedtoBeRemoved()
        {
            authRepository.Setup(s => s.GetAuthenticationType(It.IsAny<Guid>())).ReturnsAsync(new UserToken());

            var response = await authService.DisableUserMFA(new Guid("f79cd68f-2aa9-4edc-9427-742109626943"));

            Assert.That(response.IsSuccessful, Is.False);
        }

        [Test]
        public async Task ShouldFailToDisableUserMFA()
        {
            var response = await authService.DisableUserMFA(new Guid());

            Assert.That(response.IsSuccessful, Is.False);
        }

        [Test]
        public async Task ShouldSetPhoneNumberSuccessfully()
        {
            userDetailRepository.Setup(s => s.Add(It.IsAny<UserDetail>())).ReturnsAsync(true);

            var response = await authService.SetPhoneNumber(new Guid("f79cd68f-2aa9-4edc-9427-742109626943"), "0000000000");

            Assert.That(response.IsSuccessful, Is.True);
        }
        [Test]
        public async Task ShouldFailToSetPhoneNumber()
        {
            var response = await authService.SetPhoneNumber(new Guid("f79cd68f-2aa9-4edc-9427-742109626943"), "0000000000");

            Assert.That(response.IsSuccessful, Is.False);
        }


        private AuthService authService;
        private Mock<IAuthRepository> authRepository;
        private Mock<IUserDetailRepository> userDetailRepository;
        private Mock<ISmsService> smsService;
        private  Mock<IBlobService> blobService;
    }
}
