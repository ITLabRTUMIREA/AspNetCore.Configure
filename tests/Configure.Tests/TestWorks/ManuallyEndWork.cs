using Microsoft.Extensions.Logging;
using RTUITLab.AspNetCore.Configure.Configure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RTUITLab.AspNetCore.Configure.Tests.TestWorks
{
    class ManuallyEndWork : IConfigureWork
    {
        private readonly TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
        private readonly ILogger<ManuallyEndWork> logger;

        public ManuallyEndWork(ILogger<ManuallyEndWork> logger)
        {
            this.logger = logger;
        }
        public void DoneAction()
        {
            taskCompletionSource.SetResult(null);
            int iteration = 0;
            while (!taskCompletionSource.Task.IsCompleted)
            {
                Thread.SpinWait(iteration++);
            }
            logger.LogTrace($"done action end, task status is {taskCompletionSource.Task.Status}");
        }

        virtual public Task Configure()
        {
            return taskCompletionSource.Task;
        }
    }
}
