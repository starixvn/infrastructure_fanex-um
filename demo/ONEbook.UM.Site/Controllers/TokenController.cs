namespace ONEbook.UM.Site.Controllers
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using ONEbook.UM.Site.Configurations;
    using ONEbook.UM.Site.Filters;

    [UMSiteAuthorization]
    public class TokenController : Controller
    {
        private static readonly HttpClient httpClient = CreateHttpClient();

        private static HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(UMSiteConfiguration.Settings.ServiceUri);
            httpClient.Timeout = TimeSpan.FromMinutes(10);
            return httpClient;
        }

        // GET: Token
        public async Task<ActionResult> Index()
        {
            var endPoint = "api/token/GetAll";
            var method = HttpMethod.Get;
            using (var request = new HttpRequestMessage(method, new Uri(endPoint, UriKind.Relative)))
            {
                var response = await httpClient.SendAsync(request).ConfigureAwait(false);
                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    return Content(result.Replace(Environment.NewLine, "<br />"));
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
        }
    }
}