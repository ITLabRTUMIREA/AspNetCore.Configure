using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RTUITLab.AspNetCore.Configure.Behavior.Interfaces
{
    public interface IBehavior
    {
        Task OnLock(HttpContext context, RequestDelegate next);
        Task OnContinue(HttpContext context, RequestDelegate next);
    }
}
