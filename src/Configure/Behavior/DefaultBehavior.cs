using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RTUITLab.AspNetCore.Configure.Behavior.Interfaces;
using RTUITLab.AspNetCore.Configure.Invokations;

namespace RTUITLab.AspNetCore.Configure.Behavior
{
    /// <summary>
    /// Default behavior, returns 503 Service Unavailable while lock
    /// </summary>
    public class DefaultBehavior : IBehavior
    {

        public virtual Task OnContinue(HttpContext context, RequestDelegate next)
            => next(context);

        public virtual async Task OnLock(HttpContext context, RequestDelegate next, ConfigurungStatus status)
        {
            context.Response.StatusCode = 503;
            await context.Response.WriteAsync(string.Join(",", status.AllPriority.Select(p => status.DonePriority.Contains(p) ? $"{p}+" : p.ToString())));
        }
    }
}
