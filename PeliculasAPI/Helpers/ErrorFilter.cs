using Microsoft.AspNetCore.Mvc.Filters;

namespace PeliculasAPI.Helpers
{
    public class ErrorFilter: ExceptionFilterAttribute
    {
        private readonly ILogger<ErrorFilter> _logger;

        public ErrorFilter(ILogger<ErrorFilter> logger)
        {
            _logger = logger;
        }


        public override void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, context.Exception.Message);
            base.OnException(context);
        }
    }
}
