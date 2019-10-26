using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace RTUITLab.AspNetCore.Configure.Behavior
{
    /// <summary>
    /// Configured behavior, based on actions
    /// </summary>
    public class ConfiguredBehavior : DefaultBehavior
    {
        public Func<HttpContext, RequestDelegate, Task> ContinueAction { get; set; }
        public Func<HttpContext, RequestDelegate, Task> LockAction { get; set; }

        public override Task OnContinue(HttpContext context, RequestDelegate next)
            => (ContinueAction ?? base.OnContinue)(context, next);

        public override Task OnLock(HttpContext context, RequestDelegate next)
            => (LockAction ?? base.OnLock)(context, next);

    }
}
