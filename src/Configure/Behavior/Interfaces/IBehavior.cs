using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RTUITLab.AspNetCore.Configure.Behavior.Interfaces
{
    /// <summary>
    /// Determines, what actions will be preformed when locking or upon completion of all work
    /// </summary>
    public interface IBehavior
    {
        /// <summary>
        /// That action will be invoked on Path <see cref="WorkHandlePath.Lock"></see>
        /// </summary>
        /// <param name="context">Http context of request</param>
        /// <param name="next">Next middleware action</param>
        Task OnLock(HttpContext context, RequestDelegate next);
        /// <summary>
        /// That action will be invoked on Path <see cref="WorkHandlePath.Continue"></see>
        /// </summary>
        /// <param name="context">Http context of request</param>
        /// <param name="next">Next middleware action</param>
        Task OnContinue(HttpContext context, RequestDelegate next);
    }
}
