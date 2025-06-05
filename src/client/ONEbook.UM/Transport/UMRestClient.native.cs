namespace ONEbook.UM.Transport;

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Configurations;
using Helpers;
using Newtonsoft.Json;

public class UMRestClient : IUMRestClient
{
    private const int DefaultTimeOut = 10;
    private const string Splash = "/";
    private readonly HttpClient httpClient = new();
    private readonly UMClientConfiguration configuration;
    private readonly IPHelper ipHelper;

    public UMRestClient(UMClientConfiguration configuration = null, IPHelper ipHelper = null)
    {
        this.configuration = configuration ?? UMClientManager.UMClientConfigs;
        this.ipHelper = ipHelper ?? new IPHelperImpl();
        httpClient.BaseAddress = new Uri(this.configuration.ServiceUri + Splash);
        httpClient.Timeout = TimeSpan.FromMinutes(DefaultTimeOut);
    }

    public T ExecuteGet<T>(string endPoint) where T : new()
        => ExecuteAsync<T>(endPoint, HttpMethod.Get, null).GetAwaiter().GetResult();

    public T ExecutePost<T>(string endPoint, object data = null) where T : new()
        => ExecuteAsync<T>(endPoint, HttpMethod.Post, data).GetAwaiter().GetResult();

    public Task<T> ExecuteGetAsync<T>(string endPoint)
        => ExecuteAsync<T>(endPoint, HttpMethod.Get, null);

    public Task<T> ExecutePostAsync<T>(string endPoint, object data = null)
        => ExecuteAsync<T>(endPoint, HttpMethod.Post, data);

    private async Task<T> ExecuteAsync<T>(string endPoint, HttpMethod method, object data)
    {
        ValidateEndPoint(endPoint);

        httpClient.DefaultRequestHeaders.Authorization = BuildAuthorizationHeader(new Uri(httpClient.BaseAddress + endPoint), method);

        using var request = new HttpRequestMessage(method, new Uri(endPoint, UriKind.Relative));
        if (data != null)
        {
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            request.Content = content;
        }

        var response = await httpClient.SendAsync(request).ConfigureAwait(false);
        var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        if (response.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<T>(result);
        }

        if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
        {
            await Task.FromException(new Exception(result)).ConfigureAwait(false);
        }

        throw new InvalidOperationException(
            string.Join(
                "\r\n",
                $"Status code: {response.StatusCode}",
                $"Content: {result}",
                $"ErrorMessage: {response.ReasonPhrase}"));
    }

    private static void ValidateEndPoint(string endPoint)
    {
        if (string.IsNullOrWhiteSpace(endPoint))
        {
            throw new ArgumentNullException(nameof(endPoint));
        }
    }

    private AuthenticationHeaderValue BuildAuthorizationHeader(Uri url, HttpMethod method)
    {
        var requestUri = HttpUtility.UrlEncode(url.OriginalString.ToLowerInvariant());
        var requestHttpMethod = method.ToString();
        var authHeader = HttpClientHelper.GenerateAuthorizationHeader(configuration, requestUri, requestHttpMethod, ipHelper.GetMachineIP());

        return new AuthenticationHeaderValue("amx", authHeader);
    }
}