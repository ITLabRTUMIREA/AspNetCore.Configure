using Microsoft.Extensions.Logging;
using RTUITLab.AspNetCore.Configure.Configure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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
        public bool IsDone { get; private set; }
        override public async Task Configure(CancellationToken cancellationToken)
        {
            IsStarted = true;
            await base.Configure(cancellationToken);
            IsDone = true;
        }
    }
    interface IPrioriy { }
    class Priority1 : IPrioriy { }
    class Priority2 : IPrioriy { }
    class Priority3 : IPrioriy { }
}
