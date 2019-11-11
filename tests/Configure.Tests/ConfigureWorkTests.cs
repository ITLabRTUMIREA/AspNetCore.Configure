using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using RTUITLab.AspNetCore.Configure.Configure;
using RTUITLab.AspNetCore.Configure.Configure.Interfaces;
using RTUITLab.AspNetCore.Configure.Shared.Interfaces;
using RTUITLab.AspNetCore.Configure.Tests.TestWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace RTUITLab.AspNetCore.Configure.Tests
{
    public class ConfigureWorkTests : BaseTests
    {
        public ConfigureWorkTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        private TestServer CreatePriorityServerWithDefaultBehavior()
            => new TestServer(CreateDefaultWebHostBuilder()
                .ConfigureServices(s => s
                    .AddSingleton<PriorityTestWork<Priority1>>()
                    .AddSingleton<PriorityTestWork<Priority2>>()
                    .AddSingleton<PriorityTestWork<Priority3>>()
                    .AddWebAppConfigure()
                        .AddCongifure<PriorityTestWork<Priority1>>(priority: 1)
                        .AddCongifure<PriorityTestWork<Priority2>>(priority: 2)
                        .AddCongifure<PriorityTestWork<Priority3>>(priority: 3)
                ));

        [Fact]
        public async Task Configure_PriorityTasksRunСonsistently()
        {
            var server = CreatePriorityServerWithDefaultBehavior();

            var work1 = server.Services.GetRequiredService<PriorityTestWork<Priority1>>();
            var work2 = server.Services.GetRequiredService<PriorityTestWork<Priority2>>();
            var work3 = server.Services.GetRequiredService<PriorityTestWork<Priority3>>();

            var client = server.CreateClient();

            var response = await client.GetAsync("");
            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);

            Assert.False(work2.IsStarted, "work2 must be running only after work1 done");
            Assert.False(work3.IsStarted, "work3 must be running only after work2 done");

            work1.DoneAction();


            while (true)
            {
                var getter = server.Services.GetRequiredService<IWorkPathGetter>();
                getter.GetConfigureStatus(out var status);
                if (status.DonePriority.Contains(1))
                    break;
            }

            Assert.True(work1.IsStarted, "work1 must be running before work2 start");
            Assert.True(work1.IsDone, "work1 must be done before work2 start");
            
            Assert.False(work3.IsStarted, "work3 must be running only after work2 done");


            response = await client.GetAsync("");
            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);

            work2.DoneAction();

            while (true)
            {
                var getter = server.Services.GetRequiredService<IWorkPathGetter>();
                getter.GetConfigureStatus(out var status);
                if (status.DonePriority.Contains(1) && status.DonePriority.Contains(2))
                    break;
            }

            Assert.True(work1.IsStarted, "work1 must be running before work2 start");
            Assert.True(work1.IsDone, "work1 must be done before work2 start");
            Assert.True(work2.IsStarted, "work2 must be running after work1 done");
            Assert.True(work2.IsDone, "work2 must be done");

            response = await client.GetAsync("");
            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);

            work3.DoneAction();

            while (true)
            {
                var getter = server.Services.GetRequiredService<IWorkPathGetter>();
                getter.GetConfigureStatus(out var status);
                if (status.DonePriority.Contains(1) && 
                    status.DonePriority.Contains(2) &&
                    status.DonePriority.Contains(3))
                    break;
            }

            Assert.True(work1.IsStarted, "work1 must be running before work2 start");
            Assert.True(work1.IsDone, "work1 must be done before work2 start");

            Assert.True(work2.IsStarted, "work2 must be running after work1 done");
            Assert.True(work2.IsDone, "work2 must be done");

            Assert.True(work3.IsStarted, "work3 must start");
            Assert.True(work3.IsDone, "work3 must be done");

            response = await client.GetAsync("");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
