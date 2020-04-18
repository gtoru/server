using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using server.core.Api.Infrastructure;
using server.core.Infrastructure;

namespace server.core.Api.Middleware
{
    public class UnitOfWorkMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly StatusReporter _statusReporter;

        public UnitOfWorkMiddleware(RequestDelegate next, StatusReporter statusReporter)
        {
            _next = next;
            _statusReporter = statusReporter;
        }

        public async Task Invoke(HttpContext context, IUnitOfWork unitOfWork)
        {
            try
            {
                _statusReporter.AddActiveUnit();
                await _next(context);
                await unitOfWork.SaveAsync();
            }
            finally
            {
                unitOfWork.Dispose();
                _statusReporter.RemoveActiveUnt();
            }
        }
    }
}
