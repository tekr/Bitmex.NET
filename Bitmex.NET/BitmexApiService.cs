using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Bitmex.NET.Models;

namespace Bitmex.NET
{
	public class BitmexApiService : IBitmexApiService
	{
	    private readonly IBitmexAuthorization _bitmexAuthorization;
	    private readonly IBitmexApiProxy _bitmexApiProxy = new BitmexApiProxy();

        protected BitmexApiService(IBitmexAuthorization bitmexAuthorization)
		{
		    _bitmexAuthorization = bitmexAuthorization;
		}

		public async Task<TResult> Execute<TParams, TResult>(ApiActionAttributes<TParams, TResult> apiAction,
		    TParams @params, IBitmexAuthorization bitmexAuthorization = null)
		{
		    bitmexAuthorization = bitmexAuthorization ?? _bitmexAuthorization;

		    if (bitmexAuthorization == null)
		    {
		        throw new InvalidOperationException("No IBitmexAuthorization instance specified");
		    }

            return JsonConvert.DeserializeObject<TResult>(await _bitmexApiProxy.RequestAsync(bitmexAuthorization,
		        ToHttpMethod(apiAction.Method), apiAction.Action, @params));
		}

	    public void Dispose()
	    {
	        _bitmexApiProxy.Dispose();
	    }

        /// <summary>
        /// Creates a new API service instance
        ///
        /// In typical single-account scenarios, account details are passed in here via the bitmexAuthorization
        /// parameter and are omitted on calls to Execute().
        ///
        /// Alternatively they may be omitted here and passed in when calling Execute(), which enables trading
        /// on multiple accounts via one TCP connection.
        /// </summary>
        ///
        /// <param name="bitmexAuthorization">Account authorization. May be omitted here and specified on
        /// Execute() calls instead</param>
        /// 
        /// <returns>A new API service instance</returns>
	    public static IBitmexApiService CreateDefaultApi(IBitmexAuthorization bitmexAuthorization = null)
	    {
	        return new BitmexApiService(bitmexAuthorization);
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
	                throw new ArgumentOutOfRangeException();
	        }
	    }
	}
}
