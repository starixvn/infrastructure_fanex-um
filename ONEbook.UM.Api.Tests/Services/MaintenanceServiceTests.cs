namespace ONEbook.UM.Api.Tests.Services
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using NSubstitute;
    using NUnit.Framework;
    using ONEbook.UM.Api.Configurations;
    using ONEbook.UM.Api.Services.Impl;
    using Sunplus.UMControllerLibrary;

    [TestFixture]
    public class MaintenanceServiceTests
    {
        private MaintenanceService _maintenanceService;

        [SetUp]
        public void SetUp()
        {
            _maintenanceService = new MaintenanceService();
        }

        #region IsValidUMSecretToken

        [TestCase(null)]
        [TestCase("")]
        public void IsValidUMSecretToken_TokenIsNullOrEmpty_ThrowArgumentException(string token)
        {
            Assert.Throws<ArgumentException>(new TestDelegate(
                () => _maintenanceService.IsValidUMSecretToken(Arg.Any<Sites>(), token)));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void IsValidUMSecretToken_SiteIdIsNegativeOrZero_ThrowArgumentOutOfRangeException(int siteId)
        {
            string token = "Token";

            Assert.Throws<ArgumentOutOfRangeException>(new TestDelegate(
                () => _maintenanceService.IsValidUMSecretToken((Sites)siteId, token)));
        }

        [Test]
        public void IsValidUMSecretToken_WrongToken_ReturnFalse()
        {
            string token = "aaaaa";
            Sites siteId = Sites.AccountSite;

            var isValid = _maintenanceService.IsValidUMSecretToken(siteId, token);

            Assert.IsFalse(isValid);
        }

        [Test]
        public void IsValidUMSecretToken_ValidToken_ReturnTrue()
        {
            var siteId = Sites.AccountSite;
            string token = GenerateToken(siteId);

            var isValid = _maintenanceService.IsValidUMSecretToken(siteId, token);

            Assert.IsTrue(isValid);
        }

        private string GenerateToken(Sites siteId)
        {
            string key = string.Format("{0}_{1}_{2}", siteId, DateTime.UtcNow.ToShortDateString(), UnderMaintenanceApiConfiguration.Settings.SecretKey);

            return Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(key)));
        }

        #endregion IsValidUMSecretToken

        #region GetUMSecretToken

        [TestCase(0)]
        [TestCase(-1)]
        public void GetUMSecretToken_SiteIdIsNegativeOrZero_ThrowArgumentOutOfRangeException(Sites siteId)
        {
            Assert.Throws<ArgumentOutOfRangeException>(new TestDelegate(
                () => _maintenanceService.GetUMSecretToken(siteId)));
        }

        [Test]
        public void GetUMSecretToken_SiteIdIsGreaterThanZero_ReturnSecretToken()
        {
            var siteId = Sites.AccountSite;
            var expectedToken = GenerateToken(siteId);

            var actualToken = _maintenanceService.GetUMSecretToken(siteId);

            Assert.AreEqual(expectedToken, actualToken);
        }

        #endregion GetUMSecretToken

        #region GetAllUMSecretToken

        [Test]
        public void GetAllUMSecretToken_ReturnAllSiteSecretToken()
        {
            var siteIds = Enum.GetValues(typeof(Sites)).Cast<int>();

            var siteTokens = _maintenanceService.GetAllUMSecretTokens();

            var isAllSiteTokensValid = siteIds.All(siteId => siteTokens[(Sites)siteId].Token == Uri.EscapeDataString(GenerateToken((Sites)siteId)));
            Assert.IsTrue(isAllSiteTokensValid);
        }

        #endregion GetAllUMSecretToken
    }
}