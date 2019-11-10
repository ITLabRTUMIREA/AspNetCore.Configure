using Microsoft.Extensions.Logging;
using RTUITLab.AspNetCore.Configure.Configure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RTUITLab.AspNetCore.Configure.Tests.TestWorks
{
    class PriorityTestWork<T> : ManuallyEndWork
        where T : IPrioriy
    {
        public PriorityTestWork(ILogger<ManuallyEndWork> logger) : base(logger)
        {
        }

        public bool IsStarted { get; private set; }
        override public Task Configure()
        {
            IsStarted = true;
            return base.Configure();
        }
    }
    interface IPrioriy { }
    class Priority1 : IPrioriy { }
    class Priority2 : IPrioriy { }
    class Priority3 : IPrioriy { }
}
