namespace ONEbook.UM.Tests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Configurations;
    using NUnit.Framework;
    using UM.Helpers;

    [TestFixture]
    public class RestClientHelperTests
    {
        [Test]
        public void GetSurvivedRestClient_NoErrorService_ReturnFirstServiceUrlRestClient()
        {
            // Arrange
            UMClientConfiguration.ServiceUriList = new List<string>
            {
                "http://demo.com:4545/api",
                "http://demo2.com/api",
            };

            // Act
            var restClient = RestClientHelper.GetRestClient();

            // Assert
            Assert.AreEqual("http://demo.com:4545/api", restClient.BaseUrl.ToString());
        }

        ////[Test]
        ////public void GetSurvivedRestClient_FirstServiceError_ReturnSecondServiceUrlRestClient()
        ////{
        ////    // Arrange
        ////    UMClientConfiguration.ServiceUriList = new List<string>
        ////    {
        ////        "http://demo.com:4545/api",
        ////        "http://demo2.com/api",
        ////    };

        ////    RestClientHelper.KeepTrackErrorService("http://demo.com:4545/api");

        ////    // Act
        ////    var restClient = RestClientHelper.GetSurvivedRestClient();

        ////    // Assert
        ////    Assert.AreEqual("http://demo2.com/api", restClient.BaseUrl.ToString());
        ////}
    }
}