using RTUITLab.AspNetCore.Configure.Configure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RTUITLab.AspNetCore.Configure.Tests.TestWorks
{
    class WaitingWork : IConfigureWork
    {
        private readonly TimeSpan timeToWait;

        public WaitingWork(TimeSpan timeToWait)
        {
            this.timeToWait = timeToWait;
        }
        public Task Configure(CancellationToken cancellationToken)
        {
            return Task.Delay(timeToWait, cancellationToken);
        }
    }
}
