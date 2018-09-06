namespace ONEbook.UM.Helpers
{
    using System.Web;

    public class HttpContextHelper : IHttpContextHelper
    {
        public string GetSession(string key)
        {
            if (HttpContext.Current == null)
            {
                return string.Empty;
            }

            return HttpContext.Current.Session[key]?.ToString();
        }

        public void SetSession(string key, string value)
        {
            if (HttpContext.Current == null)
            {
                return;
            }

            HttpContext.Current.Session[key] = value;
        }

        public string GetValueFromQueryString(string queryKey)
        {
            if (HttpContext.Current == null)
            {
                return string.Empty;
            }

            return HttpContext.Current.Request.QueryString[queryKey];
        }
    }
}