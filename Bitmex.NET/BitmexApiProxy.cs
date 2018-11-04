using Bitmex.NET.Authorization;
using Bitmex.NET.Dtos;
using Bitmex.NET.Logging;
using Bitmex.NET.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Bitmex.NET
{
    public class BitmexApiProxy : IBitmexApiProxy
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        private readonly ConcurrentDictionary<BitmexEnvironment, HttpClient> _clientByEnv = new ConcurrentDictionary<BitmexEnvironment, HttpClient>();

        private readonly IExpiresTimeProvider _expiresTimeProvider;
        private readonly ISignatureProvider _signatureProvider;

        public BitmexApiProxy(IExpiresTimeProvider expiresTimeProvider, ISignatureProvider signatureProvider)
        {
            _expiresTimeProvider = expiresTimeProvider;
            _signatureProvider = signatureProvider;
        }

        public BitmexApiProxy() : this(new ExpiresTimeProvider(), new SignatureProvider())
        {
        }

        public async Task<string> RequestAsync<T>(IBitmexAuthorization authorization, HttpMethod method, string action, T parameters)
        {
            var queryString = (parameters as IQueryStringParams)?.ToQueryString() ?? string.Empty;
            var content = (parameters as IJsonQueryParams)?.ToJson();

            var request = new HttpRequestMessage(method, "/api/v1/" + action + (string.IsNullOrWhiteSpace(queryString) ? string.Empty : "?" + queryString))
            {
                Content = content != null ? new StringContent(content, Encoding.UTF8, "application/json") : null
            };

            Sign(authorization, request, content);

            Log.Debug($"{request.Method} {request.RequestUri}");

            var response = await GetClient(authorization.BitmexEnvironment).SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            Log.Debug($"{request.Method} {request.RequestUri.PathAndQuery} {(response.IsSuccessStatusCode ? "resp" : "errorResp")}:{responseString}");

            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    throw new BitmexApiException(JsonConvert.DeserializeObject<BitmexApiError>(responseString));
                }
                catch (JsonReaderException)
                {
                    throw new BitmexApiException(responseString);
                }
            }

            return responseString;
        }

        public void Dispose()
        {
            foreach (var client in _clientByEnv.Values)
            {
                client.Dispose();
            }
        }

        private void Sign(IBitmexAuthorization bitmexAuthorization, HttpRequestMessage request, string @params)
        {
            request.Headers.Add("api-key", bitmexAuthorization.Key ?? string.Empty);
            request.Headers.Add("api-expires", _expiresTimeProvider.Get().ToString());
            request.Headers.Add("api-signature", _signatureProvider.CreateSignature(bitmexAuthorization.Secret ?? string.Empty,
                $"{request.Method}{request.RequestUri}{_expiresTimeProvider.Get().ToString()}{@params}"));
        }

        private HttpClient GetClient(BitmexEnvironment bitmexEnvironment)
        {
            return _clientByEnv.GetOrAdd(bitmexEnvironment,
                env =>
                {
                    var httpClient = new HttpClient { BaseAddress = new Uri($"https://{Environments.Values[env]}") };

                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/javascript"));
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/javascript"));

                    return httpClient;
                });
        }
    }
}
