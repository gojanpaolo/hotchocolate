using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace HotChocolate.Caching;

public interface IQueryResultCache
{
    Task TryCacheQueryResult(IRequestContext context, CancellationToken cancellationToken);
}

internal class HttpQueryResultCache : IQueryResultCache
{
    public Task TryCacheQueryResult(IRequestContext context, CancellationToken cancellationToken)
    {
        IHttpContextAccessor accessor = context.Services.GetRequiredService<IHttpContextAccessor>();

        accessor.HttpContext.Response.Headers.TryAdd(HeaderNames.CacheControl, "public");

        return Task.CompletedTask;
    }
}

public static class HttpQueryResultCacheRequestExecutorBuilderExtensions
{
    public static IRequestExecutorBuilder AddHttpQueryResultCache(this IRequestExecutorBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.Services.AddHttpContextAccessor();

        return builder.AddQueryResultCache<HttpQueryResultCache>();
    }

   public static IRequestExecutorBuilder AddQueryResultCache<T>(this IRequestExecutorBuilder builder)
       where T : class, IQueryResultCache
   {
       if (builder is null)
       {
           throw new ArgumentNullException(nameof(builder));
       }

        builder.Services.AddTransient<IQueryResultCache, T>();

        return builder;
   }
}
