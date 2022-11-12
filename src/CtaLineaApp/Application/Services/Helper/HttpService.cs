using CtaLineaApp.Application.Model.Account;
using CtaLineaApp.Application.Services.Helper;
using CtaLineaApp.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CtaLineaApp.Application.Services.Helper
{
    public class HttpService
        : IHttpService
    {
        private HttpClient _httpClient;
        private NavigationManager _navigationManager;
        private ILocalStorageService _localStorageService;
        private IConfiguration _configuration;

        public HttpService(
            HttpClient httpClient,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService,
            IConfiguration configuration
        )
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
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
            return await SendRequest<T>(request);
        }

        public async Task Post(string uri, object value)
        {
            var request = createRequest(HttpMethod.Post, uri, value);
            await sendRequest(request);
        }

        public async Task<T?> Post<T>(string uri, object value)
        {
            var request = createRequest(HttpMethod.Post, uri, value);
            return await SendRequest<T>(request);
        }

        public async Task Put(string uri, object value)
        {
            var request = createRequest(HttpMethod.Put, uri, value);
            await sendRequest(request);
        }

        public async Task<T?> Put<T>(string uri, object value)
        {
            var request = createRequest(HttpMethod.Put, uri, value);
            return await SendRequest<T>(request);
        }

        public async Task Delete(string uri)
        {
            var request = createRequest(HttpMethod.Delete, uri);
            await sendRequest(request);
        }

        public async Task<T?> Delete<T>(string uri)
        {
            var request = createRequest(HttpMethod.Delete, uri);
            return await SendRequest<T>(request);
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

        private async Task sendRequest(HttpRequestMessage request)
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

            await HandleErrors(response);
        }

        private async Task<T?> SendRequest<T>(
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

            await HandleErrors(response);

            var options = new JsonSerializerOptions();
            options.PropertyNameCaseInsensitive = true;
            options.Converters.Add(new StringConverter());
            return await response.Content.ReadFromJsonAsync<T>(options);
        }
        public async Task<HttpResponseMessage> SendBaserequestAsync (
            HttpRequestMessage request)
        {
            await AddJwtHeader(request);

            // send request
            return await _httpClient.SendAsync(request);
        }

        private async Task AddJwtHeader(
            HttpRequestMessage request)
        {
            if (request == null) return;

            // add jwt auth header if user is logged in and request is to the api url
            var user = await _localStorageService.GetItem<UserModel>(Constants.LoalStorageKey_User);
            var isApiUrl = !(request.RequestUri?.IsAbsoluteUri ?? false);
            if (user != null && isApiUrl)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
            }
        }

        private async Task HandleErrors(
            HttpResponseMessage response)
        {
            // throw exception on error response
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                if (error != null)
                {
                    throw new Exception(error["message"]);
                }
                else
                {
                    throw new Exception("Errore sconosciuto");
                }
            }
        }
    }
}
