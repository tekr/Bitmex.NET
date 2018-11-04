using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bitmex.NET
{
	public interface IBitmexApiProxy : IDisposable
	{
	    Task<string> RequestAsync<T>(IBitmexAuthorization authorization, HttpMethod method, string action, T parameters);
    }
}
