namespace ONEbook.UM.Api.Tests.Controllers
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
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
        }

        [Fact]
        public void Validate_RequestIsNotNull_ReturnIsValidTokenInfo()
        {
            // Arrange
            tokenService.IsValidUMToken(Arg.Is(Sites._789yAgencySite), Arg.Is("1234")).Returns(true);

            // Act
            controller.Validate((int)Sites._789yAgencySite, "1234");

            // Assert
            tokenService.Received().IsValidUMToken(Sites._789yAgencySite, "1234");
        }

        [Fact]
        public void GetAll_Always_GetAllUMTokens()
        {
            // Arrange
            var siteTokens = new Dictionary<Sites, SiteToken> { { Sites.Test, new SiteToken { SiteId = 9999999, Token = "1234" } } };
            tokenService.GetAllUMTokens().Returns(siteTokens);

            // Act
            controller.GetAll();

            // Assert
            tokenService.Received().GetAllUMTokens();
        }
    }
}