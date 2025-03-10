namespace ONEbook.UM.DependencyInjection;

using Helpers;

public class EmptyHttpContextHelper() : IHttpContextHelper
{
    public string GetSession(string key)
    {
        return string.Empty;
    }

    public void SetSession(string key, string value)
    {
    }

    public string GetValueFromQueryString(string queryKey)
    {
        return string.Empty;
    }
}