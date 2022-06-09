namespace ONEbook.UM.Helpers
{
    using System.Threading.Tasks;

    public interface IUMRestClient
    {
        T ExecuteGet<T>(string endPoint) where T : new();

        T ExecutePost<T>(string endPoint, object data = null) where T : new();

        Task<T> ExecuteGetAsync<T>(string endPoint);

        Task<T> ExecutePostAsync<T>(string endPoint, object data = null);
    }
}