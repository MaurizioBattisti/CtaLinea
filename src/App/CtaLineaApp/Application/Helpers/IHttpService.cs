namespace CtaLineaApp.Application.Helpers
{
    public interface IHttpService
    {
        Task<HttpResponseMessage> Get(string uri);
        Task<T?> Get<T>(string uri);
        Task<T?> Get<T, TBadRequestREsult>(string uri);

        Task<HttpResponseMessage> Post(string uri, object value);
        Task Post<TBadRequestREsult>(string uri, object value);
        Task<T?> Post<T, TBadRequestREsult>(string uri, object value);

        Task Put(string uri, object value);
        Task Put<TBadRequestREsult>(string uri, object value);
        Task<T?> Put<T, TBadRequestREsult>(string uri, object value);

        Task Delete(string uri);
        Task Delete<TBadRequestREsult>(string uri);
        Task<T?> Delete<T, TBadRequestREsult>(string uri);


        Task<HttpResponseMessage> SendBaserequestAsync(
            HttpRequestMessage request);
    }
}
