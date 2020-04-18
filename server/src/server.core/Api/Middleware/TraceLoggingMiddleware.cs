using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace server.core.Api.Middleware
{
    public class TraceLoggingMiddleware
    {
        public const string TraceIdHeader = "x-request-id";
        private readonly RequestDelegate _next;

        public TraceLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var traceId = context.Request.Headers[TraceIdHeader];

            if (traceId == StringValues.Empty)
                traceId = Guid.NewGuid().ToString();

            using (LogContext.PushProperty("TraceId", traceId))
            {
                await _next(context);
            }
        }
    }
}
