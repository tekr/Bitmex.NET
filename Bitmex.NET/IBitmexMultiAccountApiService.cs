using System;
using System.Threading.Tasks;

namespace Bitmex.NET
{
    public interface IBitmexMultiAccountApiService : IDisposable
    {
        Task<TResult> Execute<TParams, TResult>(IBitmexAuthorization bitmexAuthorization, ApiActionAttributes<TParams, TResult> apiAction,
            TParams @params);
    }
}