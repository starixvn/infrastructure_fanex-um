namespace ONEbook.UM.Api.Tests.Controllers
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using NSubstitute;
    using ONEbook.UM.Api.Constants;
    using ONEbook.UM.Api.Controllers;
    using ONEbook.UM.Api.Models;
    using ONEbook.UM.Api.Services;
    using Xunit;

    public class TokenControllerTests
    {
        private readonly TokenController controller;
        private readonly IUMTokenService tokenService;

        public TokenControllerTests()
        {
            tokenService = Substitute.For<IUMTokenService>();
            controller = new TokenController(tokenService);
            controller.Request = new HttpRequestMessage();
            controller.Request.SetConfiguration(new HttpConfiguration());
        }

        [Fact]
        public void Validate_RequestIsNull_ReturnErrorResponse()
        {
            // Act
            var response = controller.Validate(null);

            // Assert
            Assert.Equal(HttpStatusCode.PreconditionFailed, response.StatusCode);
            Assert.Contains("Token Validation Request cannot be null!", response.Content.ReadAsStringAsync().Result);
        }

        [Fact]
        public void Validate_RequestIsNotNull_ReturnIsValidTokenInfo()
        {
            // Arrange
            var tokenRequest = new TokenValidationRequest { SiteId = Sites.Agent, Token = "1234" };
            tokenService.IsValidUMToken(Arg.Is(Sites.Agent), Arg.Is("1234")).Returns(true);

            // Act
            var response = controller.Validate(tokenRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("true", response.Content.ReadAsStringAsync().Result);
        }

        [Fact]
        public void GetAll_Always_GetAllUMTokens()
        {
            // Arrange
            var siteTokens = new Dictionary<Sites, SiteToken> { { Sites.Agent, new SiteToken { SiteId = 3300000, Token = "1234" } } };
            tokenService.GetAllUMTokens().Returns(siteTokens);

            // Act
            var response = controller.GetAll();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("{\"ABB688\":{\"SiteId\":1,\"Token\":\"1234\"}}", response.Content.ReadAsStringAsync().Result);
        }
    }
}