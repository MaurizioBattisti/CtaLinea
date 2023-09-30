using CtaLineaApp.Application.Model.Account;
using CtaLineaApp.Application.Model.Utility;
using CtaLineaApp.Application.Services.Utility;
using CtaLineaApp.Helpers;
using Microsoft.AspNetCore.Components;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace CtaLineaApp.Application.Services.Helper
{
    public class HttpService
        : IHttpService
    {
        private readonly  HttpClient _httpClient;
        private readonly  NavigationManager _navigationManager;
        private readonly  ILocalStorageService _localStorageService;
        private readonly  IGlobalFiltersService _FilterData;
        private readonly  IConfiguration _configuration;
        private readonly IApplicationSettings _appSettings;

        public HttpService(
            IApplicationSettings appSettings,
            HttpClient httpClient,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService,
            IGlobalFiltersService filterData,
            IConfiguration configuration
        )
        {
            _appSettings = appSettings;
            _httpClient = httpClient;
            // imposta il timeout 
			// _httpClient.Timeout = TimeSpan.FromSeconds(Constants.HttpTimeOut_Seconds);

			_navigationManager = navigationManager;
            _localStorageService = localStorageService;
            _FilterData = filterData;
            _configuration = configuration;
        }

        public async Task<HttpResponseMessage> Get (string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
			return await this.SendBaserequestAsync (request);
        }
        public async Task<T?> Get<T>(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return await SendRequest<T, string>(request);
        }
        public async Task<T?> Get<T, TBadRequestREsult>(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return await SendRequest<T, TBadRequestREsult>(request);
        }

        public async Task<HttpResponseMessage> Post(string uri, object value)
        {
            var request = createRequest(HttpMethod.Post, uri, value);
            var response = await this.SendBaserequestAsync(request);
            await HandleErrors<string>(response);
            return response;
        }
        public async Task Post<TBadRequestREsult>(string uri, object value)
        {
            var request = createRequest(HttpMethod.Post, uri, value);
            await sendRequest<TBadRequestREsult>(request);
        }
        public async Task<T?> Post<T, TBadRequestREsult>(string uri, object value)
        {
            var request = createRequest(HttpMethod.Post, uri, value);
            return await SendRequest<T, TBadRequestREsult>(request);
        }

        public async Task Put(string uri, object value)
        {
            var request = createRequest(HttpMethod.Put, uri, value);
            await sendRequest<string> (request);
        }
        public async Task Put<TBadRequestREsult> (string uri, object value)
        {
            var request = createRequest(HttpMethod.Put, uri, value);
            await sendRequest<TBadRequestREsult>(request);
        }
        public async Task<T?> Put<T, TBadRequestREsult>(string uri, object value)
        {
            var request = createRequest(HttpMethod.Put, uri, value);
            return await SendRequest<T, TBadRequestREsult>(request);
        }

        public async Task Delete(string uri)
        {
            var request = createRequest(HttpMethod.Delete, uri);
            await sendRequest<string>(request);
        }

        public async Task Delete<TBadRequestREsult> (string uri)
        {
            var request = createRequest(HttpMethod.Delete, uri);
            await sendRequest<TBadRequestREsult>(request);
        }

        public async Task<T?> Delete<T, TBadRequestREsult>(string uri)
        {
            var request = createRequest(HttpMethod.Delete, uri);
            return await SendRequest<T, TBadRequestREsult>(request);
        }

        // helper methods

        private HttpRequestMessage createRequest(HttpMethod method, string uri, object value = null)
        {
            var request = new HttpRequestMessage(method, uri);
            if (value != null)
            {
                request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
            }
            return request;
        }

        private async Task sendRequest<TBadRequestREsult> (HttpRequestMessage request)
        {
            await AddJwtHeader(request);

            // send request
            using var response = await _httpClient.SendAsync(request);

            // auto logout on 401 response
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _navigationManager.NavigateTo("account/logout");
                return;
            }

            await HandleErrors<TBadRequestREsult>(response);
        }

        private async Task<T?> SendRequest<T, TBadRequestREsult>(
            HttpRequestMessage request)
        {
            // send request
            using var response = await SendBaserequestAsync(request);

            // auto logout on 401 response
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _navigationManager.NavigateTo("account/logout");
                return default;
            }

            await HandleErrors<TBadRequestREsult>(response);

            var options = new JsonSerializerOptions();
            options.PropertyNameCaseInsensitive = true;
            options.Converters.Add(new StringConverter());
            return await response.Content.ReadFromJsonAsync<T>(options);
        }
        public async Task<HttpResponseMessage> SendBaserequestAsync (
            HttpRequestMessage request)
        {
            await AddJwtHeader(request);
			// imposta il timeout 
			// _httpClient.Timeout = TimeSpan.FromSeconds(Constants.HttpTimeOut_Seconds);

			// send request
			return await _httpClient.SendAsync( request );
        }

        private async Task AddJwtHeader(
            HttpRequestMessage request)
        {
            if (request == null) return;

            // aggiunge gli header dei filtir globali se diversi da nullo
            if (_FilterData.PeriodStartDate != null)
            {
                request.Headers.Add(Constants.RequestHeader_PeriodStartDate,
					string.Format("{0:yyyy-MM-dd}",
                    _FilterData.PeriodStartDate));
            }
            if (_FilterData.PerioEndDate != null)
            {
                request.Headers.Add(Constants.RequestHeader_PeriodEndDate,
					string.Format("{0:yyyy-MM-dd}",
                    _FilterData.PerioEndDate));
            }
            if (_FilterData.ContractId != null)
            {
                request.Headers.Add(Constants.RequestHeader_ContractId,
                    string.Format("{0}",
                    _FilterData.ContractId));
            }

            // add jwt auth header if user is logged in and request is to the api url
            var user = await _localStorageService.GetItem<UserModel>(Constants.LoalStorageKey_User);
            var isApiUrl = !(request.RequestUri?.IsAbsoluteUri ?? false);
            if (user != null && isApiUrl)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
            }
        }

        private async Task HandleErrors<TBadRequestREsult>(
            HttpResponseMessage response)
        {
            // throw exception on error response
            if (!response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions();
                options.PropertyNameCaseInsensitive = true;
                options.Converters.Add(new StringConverter());

                // gestisce in modo ad hoc il Bad Request
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    // tenta di deserializzare il tipo indicato dal chiamante
                    try
                    {
                        var checkResult = await response.Content.ReadFromJsonAsync<TBadRequestREsult>(options);
                        if (checkResult != null)
                        {
                            throw new BadRequestException<TBadRequestREsult>("Bad Request", checkResult);
                        }
                    }
                    catch
                    {
                        // si mangia l'errore e ci riprova
                    }
                }

                LineaProblemDetailsException? probelmExc = null;
                try
                {
                    var myPRob = await response.Content.ReadFromJsonAsync<LineaProblemDetails>(options);
                    if (myPRob != null)
                    {
                        probelmExc = new LineaProblemDetailsException(myPRob.Detail, myPRob);
                    }
                }
                catch 
                {
                    // si mangia l'errore e ci riprova
                }
                if (probelmExc  == null)
                {
                    try
                    {
                        var strPRoblem = await response.Content.ReadAsStringAsync();
                        if (strPRoblem != null)
                        {
                            probelmExc = new LineaProblemDetailsException(strPRoblem);
                        }
                    }
                    catch (Exception ex)
                    {
                        probelmExc = new LineaProblemDetailsException("Errore sconosciutoo", ex);
                    }
                }
                if (probelmExc == null) probelmExc= new LineaProblemDetailsException("Errore sconosciutoo");
                throw probelmExc;
            }
        }
    }
}
