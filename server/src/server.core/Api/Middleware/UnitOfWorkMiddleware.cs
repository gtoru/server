using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using server.core.Infrastructure;

namespace server.core.Api.Middleware
{
    public class UnitOfWorkMiddleware
    {
        private readonly RequestDelegate _next;

        public UnitOfWorkMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUnitOfWork unitOfWork)
        {
            await _next(context);
            await unitOfWork.SaveAsync();
        }
    }
}
