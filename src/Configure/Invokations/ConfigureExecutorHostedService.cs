using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using RTUITLab.AspNetCore.Configure.Behavior;
using RTUITLab.AspNetCore.Configure.Configure.Interfaces;
using RTUITLab.AspNetCore.Configure.Configure;
using RTUITLab.AspNetCore.Configure.Shared.Interfaces;

namespace RTUITLab.AspNetCore.Configure.Invokations
{
    /// <summary>
    /// Service for running configure works in background
    /// </summary>
    public class ConfigureExecutorHostedService : BackgroundService, IWorkPathGetter
    {
        private readonly ConfigureBuilder configureBuilder;
        private readonly ILogger<ConfigureExecutorHostedService> logger;
        private readonly IServiceProvider serviceProvider;

        private readonly List<IConfigurationWorkBuilder> builders;

        private List<WorkItem> workItems = new List<WorkItem>();

        public ConfigureExecutorHostedService(
            ConfigureBuilder configureBuilder,
            IServiceProvider serviceProvider,
            ILogger<ConfigureExecutorHostedService> logger)
        {
            this.configureBuilder = configureBuilder;
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            builders = configureBuilder.Builders.ToList();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var lastWorkId = 0;
            workItems = configureBuilder
                .Builders
                .Select(builder =>
                {
                    var scope = serviceProvider.CreateScope();
                    return new
                    {
                        id = Interlocked.Increment(ref lastWorkId),
                        scope,
                        builder,
                        configureWork = scope.ServiceProvider.GetService(builder.ConfigureWorkType) as IConfigureWork
                    };
                })
                .Where(b => b.configureWork != null)
                .Select(b =>new WorkItem {
                    Id = b.id,
                    WorkTask = b.configureWork.Configure().ContinueWith(t => b.id, stoppingToken),
                    Builder = b.builder,
                    Work = b.configureWork,
                    ServiceScope = b.scope
                })
                .ToList();
            var tasks = workItems.Select(w => w.WorkTask).ToList();
            while (tasks.Count != 0)
            {
                var completed = await Task.WhenAny(tasks);
                tasks.Remove(completed);

                logger.LogInformation(BuildStatus());
                var workItem = workItems.SingleOrDefault(w => w.Id == completed.Result);
                workItem.ServiceScope?.Dispose();
            }
        }


        private string BuildStatus()
        {
            var builder = new StringBuilder();
            builder.AppendLine("CURRENT CONFIGURE BUILD STATUS: ");
            foreach (var workItem in workItems)
            {
                builder.AppendLine(
                    $"{TaskIcon(workItem.WorkTask)} Work {workItem.Work.GetType().FullName} :: {workItem.Builder.WorkHandlePath} path");
                if (workItem.WorkTask.IsCanceled)
                    builder.AppendLine("    Work cancelled");

                if (!workItem.WorkTask.IsFaulted) continue;
                builder.AppendLine("    Work faulted");
                var exception = workItem.WorkTask.Exception ?? new Exception("Exception in task is null, what?");
                builder.AppendLine(workItem.WorkTask.Exception?.GetType().FullName);
                builder.AppendLine(workItem.WorkTask.Exception?.Message);
                if (exception is AggregateException aggregate)
                    foreach (var inner in aggregate.InnerExceptions)
                    {
                        builder.AppendLine($"       {inner.GetType().FullName}");
                        builder.AppendLine($"       {inner.Message}");
                        builder.AppendLine($"       {inner.StackTrace}");
                    }
                else
                {
                    builder.AppendLine(exception.StackTrace);
                }
            }
            return builder.ToString();
        }

        private static char TaskIcon(Task task)
        {
            switch (task.Status)
            {
                case TaskStatus.Canceled:
                    return '-';
                case TaskStatus.Faulted:
                    return 'X';
                case TaskStatus.RanToCompletion:
                    return '+';
                default:
                    return '~';
            }
        }

        public WorkHandlePath GetHandlePath()
        {
            logger.LogTrace("GetHandlePath");
            return workItems
                .Where(wi => !wi.WorkTask.IsCompleted)
                .Select(wi => wi.Builder.WorkHandlePath)
                .DefaultIfEmpty(WorkHandlePath.Continue)
                .Max();
        }
    }
}
