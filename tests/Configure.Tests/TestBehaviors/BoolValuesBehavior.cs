using Microsoft.AspNetCore.Http;
using RTUITLab.AspNetCore.Configure.Behavior.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RTUITLab.AspNetCore.Configure.Tests.TestBehaviors
{
    class BoolValuesBehavior : IBehavior
    {
        public bool LockValue { get; set; }
        public bool ContinueValue { get; set; }

        public Task OnContinue(HttpContext context, RequestDelegate next)
        {
            ContinueValue = true;
            return next(context);
        }

        public Task OnLock(HttpContext context, RequestDelegate next)
        {
            LockValue = true;
            return next(context);
        }
    }
}
