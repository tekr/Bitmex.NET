using System.Threading.Tasks;

namespace Bitmex.NET
{
	public class BitmexApiService : IBitmexApiService
	{
	    private readonly IBitmexAuthorization _bitmexAuthorization;
	    private readonly IBitmexMultiAccountApiService _apiService = BitmexMultiAccountApiService.CreateDefaultApi();

        protected BitmexApiService(IBitmexAuthorization bitmexAuthorization)
		{
		    _bitmexAuthorization = bitmexAuthorization;
		}

		public Task<TResult> Execute<TParams, TResult>(ApiActionAttributes<TParams, TResult> apiAction,
		    TParams @params)
		{
		    return _apiService.Execute(_bitmexAuthorization, apiAction, @params);
		}

	    public void Dispose()
	    {
	        _apiService.Dispose();
	    }

	    public static IBitmexApiService CreateDefaultApi(IBitmexAuthorization bitmexAuthorization)
	    {
	        return new BitmexApiService(bitmexAuthorization);
	    }
	}
}
