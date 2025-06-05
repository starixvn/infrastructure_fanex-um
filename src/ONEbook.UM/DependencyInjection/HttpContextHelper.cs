namespace ONEbook.UM.DependencyInjection;

using Helpers;
using Microsoft.AspNetCore.Http;

public class HttpContextHelper(IHttpContextAccessor httpContextAccessor) : IHttpContextHelper
{
    public string GetSession(string key)
    {
        try
        {
            return httpContextAccessor?.HttpContext?.Session.GetString(key);
        }
        catch
        {
            return string.Empty;
        }
    }

    public void SetSession(string key, string value)
    {
        try
        {
            httpContextAccessor?.HttpContext?.Session.SetString(key, value);
        }
        catch
        {
        }
    }

    public string GetValueFromQueryString(string queryKey)
    {
        try
        {
            var request = httpContextAccessor?.HttpContext?.Request;
            return request?.Query[queryKey].ToString();
        }
        catch
        {
            return string.Empty;
        }
    }
}