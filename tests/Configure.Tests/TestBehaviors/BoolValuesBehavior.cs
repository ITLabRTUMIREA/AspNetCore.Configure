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
        private readonly TestValuesStorage valuesStorage;

        public BoolValuesBehavior(TestValuesStorage valuesStorage)
        {
            this.valuesStorage = valuesStorage;
        }

        public Task OnContinue(HttpContext context, RequestDelegate next)
        {
            valuesStorage.ContinueValue = true;
            return next(context);
        }

        public Task OnLock(HttpContext context, RequestDelegate next)
        {
            valuesStorage.LockValue = true;
            return next(context);
        }
    }
}
