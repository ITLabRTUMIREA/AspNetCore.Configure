using Microsoft.Extensions.DependencyInjection;
using RTUITLab.AspNetCore.Configure.Configure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RTUITLab.AspNetCore.Configure.Invokations
{
    internal class WorkItem
    {
        public int Id { get; set; }
        public Task ConfigureTask { get; private set; }
        public TaskStatus Status => ConfigureTask?.Status ?? TaskStatus.Created;
        public IConfigurationCase Builder { get; set; }
        public IConfigureWork Work { get; set; }
        public IServiceScope ServiceScope { get; set; }

        public void Start()
        {
            ConfigureTask = Work.Configure();
        }
        public Task<int> GetInvokeTask(CancellationToken cancellationToken) => ConfigureTask.ContinueWith(t => Id, cancellationToken);
    }
}
