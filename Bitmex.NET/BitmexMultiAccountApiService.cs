using System;
using System.Net.Http;
using System.Threading.Tasks;
using Bitmex.NET.Models;
using Newtonsoft.Json;

namespace Bitmex.NET
{
    public class BitmexMultiAccountApiService : IBitmexMultiAccountApiService
    {
        private readonly IBitmexApiProxy _bitmexApiProxy = new BitmexApiProxy();

        public async Task<TResult> Execute<TParams, TResult>(IBitmexAuthorization bitmexAuthorization,
            ApiActionAttributes<TParams, TResult> apiAction, TParams @params)
        {
            return JsonConvert.DeserializeObject<TResult>(await _bitmexApiProxy.RequestAsync(bitmexAuthorization,
                ToHttpMethod(apiAction.Method), apiAction.Action, @params));
        }

        public void Dispose()
        {
            _bitmexApiProxy.Dispose();
        }

        public static IBitmexMultiAccountApiService CreateDefaultApi()
        {
            return new BitmexMultiAccountApiService();
        }

        private static HttpMethod ToHttpMethod(HttpMethods method)
        {
            switch (method)
            {
                case HttpMethods.GET:
                    return HttpMethod.Get;
                case HttpMethods.PUT:
                    return HttpMethod.Put;
                case HttpMethods.POST:
                    return HttpMethod.Post;
                case HttpMethods.DELETE:
                    return HttpMethod.Delete;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown HttpMethods value: {method}");
            }
        }
    }
}