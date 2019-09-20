namespace ONEbook.UM.Site.Controllers
{
    using System;
    using System.Web.Mvc;
    using ONEbook.UM.Site.Configurations;
    using ONEbook.UM.Site.Filters;
    using ONEbook.UM.Transport;
    using RestSharp;

    [UMSiteAuthorization]
    public class TokenController : Controller
    {
        private readonly IRestClient _restClient;

        public TokenController()
            : this(null)
        {
        }

        public TokenController(IRestClient restClient)
        {
            _restClient = restClient ?? GetRestClient();
        }

        // GET: Token
        public ActionResult Index()
        {
            var request = new RestRequest("/token/GetAll", Method.GET);

            IRestResponse response = _restClient.Execute(request);

            return Content(response.Content.Replace(Environment.NewLine, "<br />"));
        }

        private IRestClient GetRestClient()
        {
            var restClient = new RestClient(UMSiteConfiguration.Settings.ServiceUri);
            restClient.Authenticator = new UMAuthenticator();

            return restClient;
        }
    }
}