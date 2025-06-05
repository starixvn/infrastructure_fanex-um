using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONEbook.UM.Tests.Helpers;

using System.Security.Cryptography;
using Configurations;
using UM.Helpers;
using Xunit;

public class HttpClientHelperTests
{
    // Test case 1: Test the GenerateAuthorizationHeader method with valid parameters
    [Fact]
    public void GenerateAuthorizationHeader_ValidInput_ReturnsExpectedFormat()
    {
        // Arrange
        var config = new UMClientConfiguration { SecretKey = "c2VjcmV0S2V5" }; // Base64 encoded "secretKey"
        var requestUri = "/api/resource";
        var requestMethod = "GET";
        var machineIP = "192.168.1.100";

        // Act
        var authHeader = HttpClientHelper.GenerateAuthorizationHeader(config, requestUri, requestMethod, machineIP);

        // Assert
        Assert.Contains("192.168.1.100", authHeader);        
    }

    [Fact]
    public void GenerateAuthorizationHeader_ValidSecretKey_ValidSignature()
    {
        // Arrange
        var secretKey = "c2VjcmV0S2V5";
        var config = new UMClientConfiguration { SecretKey = secretKey };
        var requestUri = "/api/resource";
        var requestMethod = "GET";
        var machineIP = "192.168.1.100";

        // Act
        var authHeader = HttpClientHelper.GenerateAuthorizationHeader(config, requestUri, requestMethod, machineIP);
        var parts = authHeader.Split(':');
        var signature = parts[1];
        var nonce = parts[2];
        var timestamp = parts[3];

        var signatureRawData = $"{machineIP}{requestMethod}{requestUri}{timestamp}{nonce}";
        var secretKeyByteArray = Convert.FromBase64String(secretKey);
        var signatureBytes = Encoding.UTF8.GetBytes(signatureRawData);

        using var hmac = new HMACSHA256(secretKeyByteArray);
        var expectedSignatureBytes = hmac.ComputeHash(signatureBytes);
        var expectedSignatureBase64String = Convert.ToBase64String(expectedSignatureBytes);

        // Assert
        Assert.Equal(expectedSignatureBase64String, signature);
    }
}