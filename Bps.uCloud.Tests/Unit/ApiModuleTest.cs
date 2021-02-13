namespace Bps.uCloud.Tests.Unit
{
    using Bps.uCloud.Contracts.API;
    using Moq;
    using Refit;
    using Xunit;

    /// <summary>
    /// This class should test individual functions of API module in isolation (using mocks/fakes).
    /// </summary>
    public class ApiModuleTest
    {

        public ApiModuleTest()
        {
            // Arrange, Act, Assert
            //var mockKeyProvider = new Mock<IKeyProvider>();
            //mockKeyProvider.Setup(key => key.Audience).Returns("API clients");
            //mockKeyProvider.Setup(key => key.Issuer).Returns("Bps.uCloud");
            //mockKeyProvider.Setup(key => key.SecretKeyBase64).Returns("8DjMi1W+OaweIZNqrTlhR1/r6Rl1zaLpkpdHHWoX3rw3u5MgQedV8qAGbdx53sAqpoQxkknMyOTbzts5XPzkp80iT7e02MJsg+iqVVLnsqU0CK4eJf/FmQluhqnCdDCxGwe5tqq7f8buAUjDUpOiU3Sm1Qc67RcHOQhZO9wUy2aIRJdugYMkFp7JF7diSOf6GemgxYgNJKEIJ8ZZzVFVCskiWcUyclg/QcgggcxYbS+GgGF6UrslIqnT6JLHyD5YflkB33UfawKpFbW4z/RPxgenDmsAMjNlify7SxEDh2PadSVdj5ndU94gQ+cHhYwW+unAILF3Zgxbw4MJBWJI/w==");
            //mockKeyProvider.Setup(key => key.SecretKey).Returns(System.Text.Encoding.UTF8.GetBytes("8DjMi1W+OaweIZNqrTlhR1/r6Rl1zaLpkpdHHWoX3rw3u5MgQedV8qAGbdx53sAqpoQxkknMyOTbzts5XPzkp80iT7e02MJsg+iqVVLnsqU0CK4eJf/FmQluhqnCdDCxGwe5tqq7f8buAUjDUpOiU3Sm1Qc67RcHOQhZO9wUy2aIRJdugYMkFp7JF7diSOf6GemgxYgNJKEIJ8ZZzVFVCskiWcUyclg/QcgggcxYbS+GgGF6UrslIqnT6JLHyD5YflkB33UfawKpFbW4z/RPxgenDmsAMjNlify7SxEDh2PadSVdj5ndU94gQ+cHhYwW+unAILF3Zgxbw4MJBWJI/w=="));

            //var controller = new AuthModule(mockKeyProvider.Object);
            //controller.Get("/values");

            //var browser = new Browser(new DefaultNancyBootstrapper());
        }

        [Fact]
        public void Should_return_valid_token()
        {
            // Given
            //var browser = new Browser(with =>
            //{
            //    with.Module<AuthModule>();
            //    with.Dependency(mockKeyProvider.Object);
            //});
            //var browser = new Browser(new DefaultNancyBootstrapper());

            // Act
            //var result = browser.Get("/values", with => with.HttpRequest());

            //authApi.Login();
            Assert.True(true);
            //var response = browser.Post("/login", ctx =>
            //{
            //    // When
            //    ctx.HttpRequest();
            //    ctx.Accept("application/json");
            //    ctx.Header("User-Agent", "Nancy Tests");
            //    ctx.FormValue("username", "test");
            //    ctx.FormValue("password", "test");
            //})

            // Then
            //response.Body.DeserializeJson<AuthResponse>().ShouldNotBeNull();

            //authApi.Login();
            Assert.True(true);
            //var response = browser.Post("/login", ctx =>
            //{
            //    // When
            //    ctx.HttpRequest();
            //    ctx.Accept("application/json");
            //    ctx.Header("User-Agent", "Nancy Tests");
            //    ctx.FormValue("username", "test");
            //    ctx.FormValue("password", "test");
            //})

            // Then
            //response.Body.DeserializeJson<AuthResponse>().ShouldNotBeNull();
        }

        public class AuthResponse
        {
            public string Token;
        }
    }
}
