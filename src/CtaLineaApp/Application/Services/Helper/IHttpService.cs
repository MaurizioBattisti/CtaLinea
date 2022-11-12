namespace CtaLineaApp.Application.Services.Helper
{
    public interface IHttpService
    {
        Task<HttpResponseMessage> Get(string uri);
        Task<T?> Get<T>(string uri);
        Task Post(string uri, object value);
        Task<T?> Post<T>(string uri, object value);
        Task Put(string uri, object value);
        Task<T?> Put<T>(string uri, object value);
        Task Delete(string uri);
        Task<T?> Delete<T>(string uri);

        Task<HttpResponseMessage> SendBaserequestAsync(
            HttpRequestMessage request);
    }
}
