using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RTUITLab.AspNetCore.Configure.Behavior.Interfaces;

namespace RTUITLab.AspNetCore.Configure.Behavior
{
    /// <summary>
    /// Default behavior, returns 503 Service Unavailable while lock
    /// </summary>
    public class DefaultBehavior : IBehavior
    {

        public virtual Task OnContinue(HttpContext context, RequestDelegate next)
            => next(context);

        public virtual Task OnLock(HttpContext context, RequestDelegate next)
        {
            context.Response.StatusCode = 503;
            return Task.CompletedTask;
        }
    }
}
