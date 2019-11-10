using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using RTUITLab.AspNetCore.Configure.Configure;
using RTUITLab.AspNetCore.Configure.Configure.Interfaces;
using RTUITLab.AspNetCore.Configure.Tests.TestWorks;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
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

        private TestServer CreateSimpleServerWithWork(
            Action lockAction = null,
            Action continueAction = null)
            => new TestServer(CreateDefaultWebHostBuilder()
                .ConfigureServices(s => s
                    .AddSingleton<PriorityTestWork<Priority1>>()
                    .AddSingleton<PriorityTestWork<Priority2>>()
                    .AddSingleton<PriorityTestWork<Priority3>>()
                    .AddWebAppConfigure()
                        .SetBehavior(
                            lockAction: (c, n) => { lockAction?.Invoke(); return n(c); },
                            continueAction: (c, n) => { continueAction?.Invoke(); return n(c); })
                        .AddCongifure<PriorityTestWork<Priority1>>(priority: 1)
                        .AddCongifure<PriorityTestWork<Priority2>>(priority: 2)
                //.AddCongifure<PriorityTestWork<Priority3>>(priority: 3)
                ));

        [Fact]
        public async Task Configure_PriorityTasks()
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

            response = await client.GetAsync("");

            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);

            Assert.False(work3.IsStarted, "work3 must be running only after work2 done");

            work2.DoneAction();
            response = await client.GetAsync("");

            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);

            work3.DoneAction();
            await Task.Delay(100);

            response = await client.GetAsync("");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Configure_WorksMustBeInvokedContinously()
        {
            var lockValue = false;
            var continueValue = false;

            var server = CreateSimpleServerWithWork(
                lockAction: () => lockValue = true,
                continueAction: () => continueValue = true
            );

            var work1 = server.Services.GetRequiredService<PriorityTestWork<Priority1>>();
            var work2 = server.Services.GetRequiredService<PriorityTestWork<Priority2>>();
            var work3 = server.Services.GetRequiredService<PriorityTestWork<Priority3>>();

            var client = server.CreateClient();

            await client.GetAsync("lock");
            Assert.True(lockValue, "lock value must be changed after request by lock path behavior");
            Assert.False(continueValue, "continue value must be stay false after request by lock path behavior");

            lockValue = false;
            work1.DoneAction();
            work2.DoneAction();
            work3.DoneAction();
            await client.GetAsync("continue");
            Assert.False(lockValue, "lock value must be stay false after request by continue path behavior");
            Assert.True(continueValue, "continue value must be changed after request by continue path behavior");
        }
    }
}
