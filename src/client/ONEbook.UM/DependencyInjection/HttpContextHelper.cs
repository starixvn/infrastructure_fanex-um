#if NET5_0_OR_GREATER

namespace ONEbook.UM.DependencyInjection
{
    using Helpers;
    using Microsoft.AspNetCore.Http;

    public class HttpContextHelper(IHttpContextAccessor httpContextAccessor) : IHttpContextHelper
    {
        public string GetSession(string key)
        {
            return httpContextAccessor?.HttpContext?.Session?.GetString(key);
        }

        public void SetSession(string key, string value)
        {
            httpContextAccessor?.HttpContext?.Session?.SetString(key, value);
        }

        public string GetValueFromQueryString(string queryKey)
        {
            var request = httpContextAccessor?.HttpContext?.Request;
            return request?.Query[queryKey].ToString();
        }
    }
}

#endif