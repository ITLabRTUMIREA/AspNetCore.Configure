using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger logger;
        private int tryNum;
        public int Id { get; }
        public WorkItemStatus Status { get; private set; }
        public IConfigurationCase Builder { get; }
        public IServiceProvider ServiceProvider { get; }

        public WorkItem(
            int id, 
            IConfigurationCase builder,
            IServiceProvider serviceProvider, 
            ILogger logger)
        {
            Id = id;
            Builder = builder;
            ServiceProvider = serviceProvider;
            this.logger = logger;
        }

        public async Task<int> Run(CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    tryNum++;
                    using (var scope = ServiceProvider.CreateScope())
                    {
                        var configureWork = scope.ServiceProvider.GetRequiredService(Builder.ConfigureWorkType) as IConfigureWork;
                        await configureWork.Configure(cancellationToken);
                    }
                    Status = WorkItemStatus.Done;
                    return Id;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, $"{Builder.ConfigureWorkType.FullName} thows exception, try #{tryNum}");
                }
            }
        }
    }
}
