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

        #region IsValidUMToken

        [TestCase(null)]
        [TestCase("")]
        public void IsValidUMToken_TokenIsNullOrEmpty_ThrowArgumentException(string token)
        {
            Assert.Throws<ArgumentException>(new TestDelegate(
                () => _maintenanceService.IsValidUMToken(Arg.Any<Sites>(), token)));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void IsValidUMToken_SiteIdIsNegativeOrZero_ThrowArgumentOutOfRangeException(int siteId)
        {
            string token = "Token";

            Assert.Throws<ArgumentOutOfRangeException>(new TestDelegate(
                () => _maintenanceService.IsValidUMToken((Sites)siteId, token)));
        }

        [Test]
        public void IsValidUMToken_WrongToken_ReturnFalse()
        {
            string token = "aaaaa";
            Sites siteId = Sites.AccountSite;

            var isValid = _maintenanceService.IsValidUMToken(siteId, token);

            Assert.IsFalse(isValid);
        }

        [Test]
        public void IsValidUMToken_ValidToken_ReturnTrue()
        {
            var siteId = Sites.AccountSite;
            string token = GenerateToken(siteId);

            var isValid = _maintenanceService.IsValidUMToken(siteId, token);

            Assert.IsTrue(isValid);
        }

        private string GenerateToken(Sites siteId)
        {
            string key = string.Format("{0}_{1}_{2}", siteId, DateTime.UtcNow.ToShortDateString(), UnderMaintenanceApiConfiguration.Settings.SecretKey);

            return Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(key)));
        }

        #endregion IsValidUMToken

        #region GetUMToken

        [TestCase(0)]
        [TestCase(-1)]
        public void GetUMToken_SiteIdIsNegativeOrZero_ThrowArgumentOutOfRangeException(Sites siteId)
        {
            Assert.Throws<ArgumentOutOfRangeException>(new TestDelegate(
                () => _maintenanceService.GetUMToken(siteId)));
        }

        [Test]
        public void GetUMToken_SiteIdIsGreaterThanZero_ReturnSecretToken()
        {
            var siteId = Sites.AccountSite;
            var expectedToken = GenerateToken(siteId);

            var actualToken = _maintenanceService.GetUMToken(siteId);

            Assert.AreEqual(expectedToken, actualToken);
        }

        #endregion GetUMToken

        #region GetAllUMTokens

        [Test]
        public void GetAllUMTokens_ReturnAllSiteSecretToken()
        {
            var siteIds = Enum.GetValues(typeof(Sites)).Cast<int>();

            var siteTokens = _maintenanceService.GetAllUMTokens();

            var isAllSiteTokensValid = siteIds.All(siteId => siteTokens[(Sites)siteId].Token == Uri.EscapeDataString(GenerateToken((Sites)siteId)));
            Assert.IsTrue(isAllSiteTokensValid);
        }

        #endregion GetAllUMTokens
    }
}