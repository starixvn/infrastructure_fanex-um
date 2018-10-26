namespace ONEbook.UM.Tests.Helpers
{
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using ONEbook.UM.Helpers;
    using Xunit;

    public class IPHelperImplTests
    {
        private readonly IPHelper ipHelper;

        public IPHelperImplTests()
        {
            ipHelper = new IPHelperImpl();
        }

        [Fact]
        public void GetMachineIP_IpAddressFamilyEqualsInterNetwork_ReturnIp()
        {
            // Arrange
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var expectedIP = host.AddressList.FirstOrDefault(address => address.AddressFamily == AddressFamily.InterNetwork).ToString();

            // Act
            var ip = ipHelper.GetMachineIP();

            // Assert
            Assert.Equal(expectedIP, ip);
        }
    }
}