using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace HotChocolate.Execution.Pipeline;

public class QueryResultCacheMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    public QueryResultCacheMiddleware(RequestDelegate next,
        IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider = serviceProvider;
    }

    public async ValueTask InvokeAsync(IRequestContext context)
    {
        var caches = _serviceProvider.GetServices<IQueryResultCache>();

        // ...

        await _next(context).ConfigureAwait(false);
    }
}

