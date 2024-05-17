using AdeAuth.Services;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Services;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models;
using AdeNote.Tests.Models;
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
            tokenProvider = new Mock<ITokenProvider>();
            passwordManager = new Mock<IPasswordManager>();
            refreshTokenRepository = new Mock<IRefreshTokenRepository>();
            mfaService = new Mock<IMfaService>();
            authService.blobService = blobService.Object;
            authService.smsService = smsService.Object;
            authService.userRepository = userRepository.Object;
            authService.tokenProvider = tokenProvider.Object;
            authService.passwordManager = passwordManager.Object;
            authService.refreshTokenRepository = refreshTokenRepository.Object;
            authService.mfaService = mfaService.Object;
            authService.key = "testKey";
            authService.loginSecret = "testLoginSecret";
        }

        [Test]
        public async Task ShouldSetAuthenticatorSuccessfully()
        {
            //Arrange
            blobService.Setup(s => s.UploadImage(It.IsAny<string>(), It.IsAny<Stream>(),Infrastructure.Utilities.MimeType.png)).ReturnsAsync("test-url");
            userRepository.Setup(s => s.Update(It.IsAny<User>())).ReturnsAsync(true);
            userRepository.Setup(s => s.GetUser(It.IsAny<Guid>())).ReturnsAsync(new User("first", "lastname", "test@gmail", AuthType.local) { TwoFactorType = 0 });
            mfaService.Setup(s => s.SetupGoogleAuthenticator("Adenote", It.IsAny<string>(), It.IsAny<byte[]>()))
                .Returns(new Authenticator("SGVsbG8sIFdvcmxkIQ==,SGVsbG8sIFdvcmxkIQ==", "12345555"));
            //Act
            var response = await authService.SetAuthenticator(new Guid("f79cd68f-2aa9-4edc-9427-742109626943"), "email");

            //Assert
            Assert.That(response.IsSuccessful, Is.True);
        }

        [Test]
        public async Task ShouldFailToSetAuthenticator()
        {
            //Arrange
            blobService.Setup(s => s.UploadImage(It.IsAny<string>(), It.IsAny<Stream>(), Infrastructure.Utilities.MimeType.png)).ReturnsAsync("test-url");
            userRepository.Setup(s => s.Update(It.IsAny<User>())).ReturnsAsync(false);
            userRepository.Setup(s => s.GetUser(It.IsAny<Guid>())).ReturnsAsync(new User("first", "lastname", "test@gmail", AuthType.local) { TwoFactorType = 0 });
            mfaService.Setup(s => s.SetupGoogleAuthenticator("Adenote", It.IsAny<string>(), It.IsAny<byte[]>()))
                .Returns(new Authenticator("SGVsbG8sIFdvcmxkIQ==,SGVsbG8sIFdvcmxkIQ==", "12345555"));

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

            var response = await authService.IsAuthenticatorEnabled(new Guid("f79cd68f-2aa9-4edc-9427-742109626943"), MFAType.google);

            Assert.That(response.IsSuccessful, Is.True);
        }

        [TestCase("4b4fe122-f720-4084-b899-9daff568e67a","Invalid authenticator type")]
        [TestCase("f79cd68f-2aa9-4edc-9427-742109626943", "No two factor enabled")]
        [TestCase("00000000-0000-0000-0000-000000000000", "Invalid user id")]
        public async Task ShouldFailToCheckIfAuthenticatorEnabled(Guid userId, string error)
        {
            userRepository.Setup(s => s.GetUser(new Guid("4b4fe122-f720-4084-b899-9daff568e67a"))).ReturnsAsync(new User("first", "lastname", "test@gmail", AuthType.local) { TwoFactorType = 2 });
            var response = await authService.IsAuthenticatorEnabled(userId, MFAType.sms);

            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors[0], Is.EqualTo(error));
        }

        [Test]
        public void ShouldGenerateMFATokenSuccessfully()
        {
            tokenProvider.Setup(s => s.GenerateToken(It.IsAny<byte[]>())).Returns("accesstoken");

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

            tokenProvider.Setup(s => s.ReadToken(token, "-")).Returns(key.Split('-'));

            var response = authService.ReadDetailsFromToken(token);

            Assert.That(response.IsSuccessful, Is.True);
        }

        [Test]
        public void ShouldFailToReadDetailsFromToken()
        {
            var key = $"login-email-f79cd68f2aa94edc9427742109626943-refreshToken";

            var encodedToken = Encoding.UTF8.GetBytes(key);

            var token = Convert.ToBase64String(encodedToken);

            tokenProvider.Setup(s => s.ReadToken(token, "-")).Returns(key.Split('-'));

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
            refreshTokenRepository.Setup(s => s.GetRefreshTokenByUserId(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(new RefreshToken("refreshToken", new Guid("f79cd68f-2aa9-4edc-9427-742109626943"), DateTime.UtcNow.AddDays(2)));

            refreshTokenRepository.Setup(s => s.Update(It.IsAny<RefreshToken>())).ReturnsAsync(true);

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
            refreshTokenRepository.Setup(s => s.GetRefreshToken(It.IsAny<string>()))
               .ReturnsAsync(new RefreshToken("refreshToken", new Guid("f79cd68f-2aa9-4edc-9427-742109626943"), DateTime.UtcNow.AddDays(2)));

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
            refreshTokenRepository.Setup(s => s.GetRefreshToken(It.IsAny<string>()));
            var response = await authService.IsTokenRevoked("Token");

            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors[0], Is.EqualTo("Invalid token"));
        }
        [Test]
        public async Task ShouldDisableUserMFASucessfully()
        {
            userRepository.Setup(s => s.GetUser(It.IsAny<Guid>())).ReturnsAsync(new User("first", "lastname", "test@gmail", AuthType.local) { TwoFactorType = 1});
            userRepository.Setup(s => s.Update(It.IsAny<User>())).ReturnsAsync(true);

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
            userRepository.Setup(s => s.GetUser(It.IsAny<Guid>())).ReturnsAsync(new User("first", "lastname", "test@gmail", AuthType.local));
            userRepository.Setup(s => s.Update(It.IsAny<User>())).ReturnsAsync(true);

            tokenProvider.Setup(s => s.GenerateOTP(It.IsAny<byte[]>())).Returns(1234);

            smsService.Setup(s => s.SendSms(It.IsAny<Sms>()));

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
        private Mock<ITokenProvider> tokenProvider;
        private Mock<IPasswordManager> passwordManager;
        private Mock<IRefreshTokenRepository> refreshTokenRepository;
        private Mock<IMfaService> mfaService;
    }
}
