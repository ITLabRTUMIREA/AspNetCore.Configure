using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using RTUITLab.AspNetCore.Configure.Invokations;

namespace RTUITLab.AspNetCore.Configure.Behavior
{
    /// <summary>
    /// Configured behavior, based on actions
    /// </summary>
    public class ConfiguredBehavior : DefaultBehavior
    {
        public Func<HttpContext, RequestDelegate, Task> ContinueAction { get; set; }
        public Func<HttpContext, RequestDelegate, ConfigurungStatus, Task> LockAction { get; set; }

        public override Task OnContinue(HttpContext context, RequestDelegate next)
            => (ContinueAction ?? base.OnContinue)(context, next);

        public override Task OnLock(HttpContext context, RequestDelegate next, ConfigurungStatus status)
            => (LockAction ?? base.OnLock)(context, next, status);

    }
}
