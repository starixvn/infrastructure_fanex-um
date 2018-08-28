namespace ONEbook.UM.Helpers
{
    public interface IHttpContextHelper
    {
        string GetSession(string key);

        void SetSession(string key, string value);

        string GetIpAddress();

        string GetValueFromQueryString(string queryKey);
    }
}