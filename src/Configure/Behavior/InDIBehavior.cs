using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RTUITLab.AspNetCore.Configure.Behavior.Interfaces;
using RTUITLab.AspNetCore.Configure.Invokations;

namespace RTUITLab.AspNetCore.Configure.Behavior
{
    public class InDIBehavior<T> : IBehavior where T : class, IBehavior
    {
        public Task OnContinue(HttpContext context, RequestDelegate next)
            => context.RequestServices.GetService<T>().OnContinue(context, next);

        public Task OnLock(HttpContext context, RequestDelegate next, ConfigurungStatus status)
            => context.RequestServices.GetService<T>().OnLock(context, next, status);

    }
}
