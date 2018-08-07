using System;
using System.Threading.Tasks;

namespace Bitmex.NET
{
	public interface IBitmexApiService : IDisposable
	{
		Task<TResult> Execute<TParams, TResult>(ApiActionAttributes<TParams, TResult> apiAction, TParams @params);
	}
}
