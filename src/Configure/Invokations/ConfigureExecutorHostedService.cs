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
        private readonly ILogger<ConfigureExecutorHostedService> logger;
        private readonly IConfigurationCaseStorage caseStorage;
        private readonly IServiceProvider serviceProvider;

        private List<WorkPart> workParts;
        public ConfigureExecutorHostedService(
            IConfigurationCaseStorage caseStorage,
            IServiceProvider serviceProvider,
            ILogger<ConfigureExecutorHostedService> logger)
        {
            this.logger = logger;
            this.caseStorage = caseStorage;
            this.serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var lastWorkId = 0;
            workParts = caseStorage
                .Works
                .Select(workCase =>
                {
                    var scope = serviceProvider.CreateScope();
                    return new
                    {
                        id = Interlocked.Increment(ref lastWorkId),
                        scope,
                        workCase,
                        configureWork = scope.ServiceProvider.GetService(workCase.ConfigureWorkType) as IConfigureWork
                    };
                })
                .Where(b => b.configureWork != null)
                .GroupBy(b => b.workCase.Priority)
                .Select(g =>
                    new WorkPart
                    {
                        Priority = g.Key,
                        WorkItems = g
                        .Select(b => new WorkItem
                        {
                            Id = b.id,
                            Builder = b.workCase,
                            Work = b.configureWork,
                            ServiceScope = b.scope
                        })
                        .ToList()
                    })
                .OrderBy(wp => wp.Priority)
                .ToList();
            foreach (var item in workParts)
            {
                await HandlePart(item, stoppingToken);
            }
        }

        private async Task HandlePart(WorkPart part, CancellationToken cancellationToken)
        {
            var items = part.WorkItems;
            items.ForEach(wi => wi.Start(cancellationToken));
            var tasks = items
                .Select(w => w.GetInvokeTask(cancellationToken))
                .ToList();
            while (tasks.Count != 0)
            {
                var completed = await Task.WhenAny(tasks);
                tasks.Remove(completed);
                var logMessage = BuildStatus();
                logger.LogInformation(logMessage);
                var workItem = items.SingleOrDefault(w => w.Id == completed.Result);
                workItem.ServiceScope?.Dispose();
            }
        }

        private string BuildStatus()
        {
            var builder = new StringBuilder();
            builder.AppendLine("CURRENT CONFIGURE BUILD STATUS: ");
            for (var i = 0; i < workParts.Count; i++)
            {
                var workPart = workParts[i];
                builder.AppendLine($"{WorkPartIcon(workPart)} Prority #{workPart.Priority}");
                foreach (var workItem in workPart.WorkItems)
                {
                    builder.AppendLine(
                        $"  {TaskIcon(workItem.Status)} Work {workItem.Work.GetType().FullName} :: {workItem.Builder.WorkHandlePath} path");

                    if (workItem.ConfigureTask == null)
                        continue;

                    if (workItem.ConfigureTask.IsCanceled)
                        builder.AppendLine("    Work cancelled");
                    if (!workItem.ConfigureTask.IsFaulted)
                        continue;

                    builder.AppendLine("    Work faulted");

                    var exception = workItem.ConfigureTask.Exception ?? new Exception("Exception in task is null, what?");
                    builder.AppendLine(workItem.ConfigureTask.Exception?.GetType().FullName);
                    builder.AppendLine(workItem.ConfigureTask.Exception?.Message);
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
            }
            return builder.ToString();
        }

        private static char WorkPartIcon(WorkPart part)
        {
            return TaskIcon(part.Status);
        }

        private static char TaskIcon(TaskStatus status)
        {
            switch (status)
            {
                case TaskStatus.Faulted:
                    return 'X';
                case TaskStatus.Canceled:
                    return '-';
                case TaskStatus.RanToCompletion:
                    return '+';
                case TaskStatus.WaitingForChildrenToComplete:
                case TaskStatus.Running:
                    return '~';
                default:
                    return '.';

            }
        }

        public WorkHandlePath GetConfigureStatus(out ConfigurungStatus status)
        {
            var path = workParts
                .SelectMany(wp => wp.WorkItems)
                .Where(wi => wi.ConfigureTask?.IsCompleted != true)
                .Select(wi => wi.Builder.WorkHandlePath)
                .DefaultIfEmpty(WorkHandlePath.Continue)
                .Max();
            status = new ConfigurungStatus(
                workParts
                    .Select(wp => wp.Priority)
                    .ToArray(),
                workParts
                    .TakeWhile(wp => wp.Status == TaskStatus.RanToCompletion)
                    .Select(wp => wp.Priority)
                    .ToArray()
            );
            return path;
        }
    }
}
