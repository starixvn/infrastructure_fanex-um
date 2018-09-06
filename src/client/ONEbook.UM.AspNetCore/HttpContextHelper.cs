namespace ONEbook.UM.Helpers
{
    using Microsoft.AspNetCore.Http;

    public class HttpContextHelper : IHttpContextHelper
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public HttpContextHelper(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public string GetSession(string key)
        {
            return httpContextAccessor.HttpContext.Session.GetString(key);
        }

        public void SetSession(string key, string value)
        {
            httpContextAccessor.HttpContext.Session.SetString(key, value);
        }

        public string GetValueFromQueryString(string queryKey)
        {
            return httpContextAccessor.HttpContext.Request.Query[queryKey].ToString();
        }
    }
}