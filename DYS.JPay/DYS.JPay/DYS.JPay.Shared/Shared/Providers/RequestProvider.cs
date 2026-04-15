using DYS.JPay.Common.Dtos;
using DYS.JPay.Common.Extensions;
using DYS.JPay.Shared.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace DYS.JPay.Shared.Shared.Providers
{
    public interface IRequestProvider
    {
        void SetToken(string token);
        string BaseURI { get; set; }
        Task<BaseResponseDto> GetAsync<T>(string path, Dictionary<string, string> headers = null);
        Task<BaseResponseDto> PostAsync<T>(string path, object payload, Dictionary<string, string> headers = null);
        Task<BaseResponseDto> PostAsync<T>(string path, Dictionary<string, string> headers = null);
        Task<BaseResponseDto> PutAsync<T>(string path, object payload, Dictionary<string, string> headers = null);
        Task<BaseResponseDto> SendAsync<T>(string path, object payload = null);
        Task<BaseResponseDto> UploadAsync<T>(string path, HttpContent payload);
        Task<BaseResponseDto> DeleteAsync<T>(string path);
    }
    public class RequestProvider : IRequestProvider, IDisposable
    {
        public string BaseURI { get; set; }

        private readonly HttpClient _httpClient;
        private CancellationTokenSource _cancellationTokenSource;
        private string _token = "";
        public void SetToken(string token) => _token = token;

        /// <summary>
        /// Requestprovider
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="configuration"></param>
        public RequestProvider(
            HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(60);
        }

        /// <summary>
        /// Get async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public Task<BaseResponseDto> GetAsync<T>(string path, Dictionary<string, string> headers = null)
        {
            var api = $"{BaseURI}{path}";
            _cancellationTokenSource = new CancellationTokenSource();

            return InvokeAsync<T>(
               client => client.GetAsync(api, _cancellationTokenSource.Token),
               response => response.Content.ReadAsStringAsync(), headers);
        }

        /// <summary>
        /// Head Method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="parameters"></param>
        /// <param name="data"></param>
        /// <param name="bypassDialogs"></param>
        /// <returns></returns>
        public Task<BaseResponseDto> SendAsync<T>(string path, object payload = null)
        {
            var api = $"{BaseURI}{path}";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, api);
            if (payload != null)
            {
                var content = new StringContent(JsonExtensions.Convert(payload),
                Encoding.UTF8,
                "application/json");
            }

            return InvokeAsync<T>(
               client => client.SendAsync(request),
               response => response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Post method using json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="parameters"></param>
        /// <param name="data"></param>
        /// <param name="bypassDialogs"></param>
        /// <returns></returns>
        public Task<BaseResponseDto> PostAsync<T>(string path, object payload, Dictionary<string, string> headers = null)
        {
            var api = $"{BaseURI}{path}";

            var content = new StringContent(JsonExtensions.Convert(payload),
                Encoding.UTF8,
                "application/json");

            return InvokeAsync<T>(
               client => client.PostAsync(api, content),
               response => response.Content.ReadAsStringAsync(), headers);
        }

        public Task<BaseResponseDto> PostAsync<T>(string path, Dictionary<string, string> headers = null)
        {
            var api = $"{BaseURI}{path}";

            return InvokeAsync<T>(
               client => client.PostAsync(api, null),
               response => response.Content.ReadAsStringAsync(), headers);
        }

        /// <summary>
		/// Put method using json
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="path"></param>
		/// <param name="parameters"></param>
		/// <param name="data"></param>
		/// <param name="bypassDialogs"></param>
		/// <returns></returns>
		public Task<BaseResponseDto> PutAsync<T>(string path, object payload, Dictionary<string, string> headers = null)
        {
            var api = $"{BaseURI}{path}";

            var content = new StringContent(JsonExtensions.Convert(payload),
                Encoding.UTF8,
                "application/json");

            return InvokeAsync<T>(
               client => client.PutAsync(api, content),
               response => response.Content.ReadAsStringAsync(), headers);
        }

        /// <summary>
        /// Delete async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public Task<BaseResponseDto> DeleteAsync<T>(string path)
        {
            var api = $"{BaseURI}{path}";

            return InvokeAsync<T>(
               client => client.DeleteAsync(api),
               response => response.Content.ReadAsStringAsync());
        }


        public Task<BaseResponseDto> UploadAsync<T>(string path, HttpContent payload)
        {
            var api = $"{BaseURI}{path}";

            return InvokeAsync<T>(
               client => client.PostAsync(api, payload),
               response => response.Content.ReadAsStringAsync());
        }


        /// <summary>
        ///  Invoke Http method 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="operation"></param>
        /// <param name="actionOnResponse"></param>
        /// <param name="bypassDialogs"></param>
        /// <returns></returns>
        private async Task<BaseResponseDto> InvokeAsync<T>(
        Func<HttpClient, Task<HttpResponseMessage>> operation,
        Func<HttpResponseMessage, Task<string>> actionOnResponse,
        Dictionary<string, string> headers = null)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            _httpClient.DefaultRequestHeaders.Clear();

            //TODO FIX PASSING OF TOKEN
            //var token = await _localStorage.GetAsync("authToken");

            //if (!string.IsNullOrEmpty(token))
            //    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{token}");

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    if (item.Key.Contains("bearer"))
                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(item.Key, item.Value);
                    else
                        _httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }

            BaseResponseDto httpResponse = new BaseResponseDto();
            try
            {
                HttpResponseMessage response = await operation(_httpClient).ConfigureAwait(false);
                var httpContent = await actionOnResponse(response).ConfigureAwait(false);
                httpResponse = new BaseResponseDto(httpContent, response.StatusCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return httpResponse;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
