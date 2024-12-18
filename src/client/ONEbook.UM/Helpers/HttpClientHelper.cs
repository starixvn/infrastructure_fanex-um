namespace ONEbook.UM.Helpers;

using System;
using System.Security.Cryptography;
using System.Text;
using Configurations;

public static class HttpClientHelper
{
    public static string GenerateAuthorizationHeader(UMClientConfiguration config, string requestUri, string requestMethod, string machineIP)
    {
        // Calculate UNIX time
        var epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
        var timeSpan = DateTime.UtcNow - epochStart;
        var requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();

        // create random nonce for each request
        var nonce = Guid.NewGuid().ToString("N");

        // Creating the raw signature string
        var signatureRawData = $"{machineIP}{requestMethod}{requestUri}{requestTimeStamp}{nonce}";

        var secretKeyByteArray = Convert.FromBase64String(config.SecretKey);

        var signature = Encoding.UTF8.GetBytes(signatureRawData);

        using var hmac = new HMACSHA256(secretKeyByteArray);
        var signatureBytes = hmac.ComputeHash(signature);
        var requestSignatureBase64String = Convert.ToBase64String(signatureBytes);

        // Setting the values in the Authorization header using custom scheme (amx)
        var authHeader = $"{machineIP}:{requestSignatureBase64String}:{nonce}:{requestTimeStamp}";

        return authHeader;
    }
}