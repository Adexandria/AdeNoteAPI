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
            blobService = new Mock<IBlobService>();
            authService = new Mock<AuthService>().Object;
            userRepository = new Mock<IUserRepository>();
            smsService = new Mock<ISmsService>();
            authService.blobService = blobService.Object;
            authService.smsService = smsService.Object;
            authService.userRepository = userRepository.Object;
            authService.key = "testKey";
            authService.loginSecret = "testLoginSecret";
        }

        [Test]
        public async Task ShouldSetAuthenticatorSuccessfully()
        {
            //Arrange
            blobService.Setup(s => s.UploadImage(It.IsAny<string>(), It.IsAny<Stream>(),Infrastructure.Utilities.MimeType.png)).ReturnsAsync("test-url");
            userRepository.Setup(s => s.Add(It.IsAny<User>())).ReturnsAsync(true);

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
            userRepository.Setup(s => s.GetUser(It.IsAny<Guid>())).ReturnsAsync(new User("first", "lastname", "test@gmail",AuthType.local) { TwoFactorType = 2 });

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
            userRepository.Setup(s => s.GetUser(It.IsAny<Guid>())).ReturnsAsync(new User("first", "lastname", "test@gmail", AuthType.local) { TwoFactorType = 2});

            var response = await authService.IsAuthenticatorEnabled(new Guid("f79cd68f-2aa9-4edc-9427-742109626943"), MFAType.sms);

            Assert.That(response.IsSuccessful, Is.True);
        }

        [TestCase("f79cd68f-2aa9-4edc-9427-742109626943", "No two factor enabled")]
        [TestCase("00000000-0000-0000-0000-000000000000", "Invalid user id")]
        public async Task ShouldFailToCheckIfAuthenticatorEnabled(Guid userId, string error)
        {
            var response = await authService.IsAuthenticatorEnabled(userId, MFAType.sms);

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
            userRepository.Setup(s => s.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(new User("first", "lastname", "test@gmail", AuthType.local));

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
            userRepository.Setup(s => s.GetUser(It.IsAny<Guid>())).ReturnsAsync(new User("first", "lastname", "test@gmail", AuthType.local));
            userRepository.Setup(s => s.Remove(It.IsAny<User>())).ReturnsAsync(true);

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
            userRepository.Setup(s => s.GetUser(It.IsAny<Guid>())).ReturnsAsync(new User("first","lastname","test@gmail", AuthType.local));

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
            userRepository.Setup(s => s.Add(It.IsAny<User>())).ReturnsAsync(true);

            var response = await authService.SetPhoneNumber(new Guid("f79cd68f-2aa9-4edc-9427-742109626943"), "0000000000");

            Assert.That(response.IsSuccessful, Is.True);
        }
        [Test]
        public async Task ShouldFailToSetPhoneNumber()
        {
            var response = await authService.SetPhoneNumber(new Guid("f79cd68f-2aa9-4edc-9427-742109626943"), "0000000000");

            Assert.That(response.IsSuccessful, Is.False);
        }

        private Mock<IUserRepository> userRepository;
        private AuthService authService;
        private Mock<ISmsService> smsService;
        private  Mock<IBlobService> blobService;
    }
}
