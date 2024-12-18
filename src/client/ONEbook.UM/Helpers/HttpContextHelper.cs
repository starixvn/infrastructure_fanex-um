namespace ONEbook.UM.Helpers;

public interface IHttpContextHelper
{
    string GetSession(string key);

    void SetSession(string key, string value);

    string GetValueFromQueryString(string queryKey);
}