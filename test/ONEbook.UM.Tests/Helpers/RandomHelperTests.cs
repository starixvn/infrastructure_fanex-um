namespace ONEbook.UM.Tests.Helpers
{
    using System;
    using NUnit.Framework;
    using ONEbook.UM.Helpers;

    public class RandomHelperTests
    {
        [TestCase("")]
        [TestCase(null)]
        public void GetRandomUri_SourceUrisIsNullOrEmpty_ThrownArgumentOutOfRangeException(string sourceUris)
        {
            Assert.Throws<ArgumentOutOfRangeException>(new TestDelegate(() => RandomHelper.GetRandomUri(sourceUris)));
        }

        [Test]
        public void GetRandomUri_SourceUrisHasOneUri_ReturnFirstUri()
        {
            var uris = "http://uri1.com/";

            var actualUri = RandomHelper.GetRandomUri(uris);

            Assert.AreEqual(uris, actualUri.ToString());
        }

        [TestCase("http://uri1.com/,http://uri2.com/")]
        [TestCase("http://uri1.com/,http://uri2.com/,http://uri3.com/")]
        public void GetRandomUri_SourceUrisHasMoreThanOneUri_ReturnRandomUri(string uris)
        {
            var actualUri = RandomHelper.GetRandomUri(uris);

            Assert.IsTrue(uris.Contains(actualUri.ToString()));
        }
    }
}