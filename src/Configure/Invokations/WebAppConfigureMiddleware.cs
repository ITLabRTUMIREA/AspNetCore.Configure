using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RTUITLab.AspNetCore.Configure.Behavior;
using RTUITLab.AspNetCore.Configure.Configure;
using RTUITLab.AspNetCore.Configure.Shared.Interfaces;
using Microsoft.Extensions.Logging;

namespace RTUITLab.AspNetCore.Configure.Invokations
{
    public class WebAppConfigureMiddleware
    {


        private readonly RequestDelegate next;
        private readonly ILogger<WebAppConfigureMiddleware> logger;

        public WebAppConfigureMiddleware(RequestDelegate next, ILogger<WebAppConfigureMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }


        public Task InvokeAsync(HttpContext context)
        {
            var workStatusGetter = context.RequestServices.GetService<IWorkPathGetter>();
            var configureBuilder = context.RequestServices.GetService<ConfigureBuilder>();

            var workStatus = workStatusGetter.GetHandlePath();
            switch (workStatus)
            {
                case WorkHandlePath.Lock:
                    logger.LogTrace("Use lock path");
                    return configureBuilder.Behavior.OnLock(context, next);
                case WorkHandlePath.Continue:
                    logger.LogTrace("Use continue path");
                    return configureBuilder.Behavior.OnContinue(context, next);
                default:
                    throw new ArgumentOutOfRangeException(nameof(WorkHandlePath));
            }
        }
    }
    public static class WebAppConfigureMiddlewareExtensions
    {
        public static IApplicationBuilder UseWebAppConfigure(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WebAppConfigureMiddleware>();
        }
    }
}
