using Microsoft.AspNetCore.Hosting;
using System;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using RTUITLab.AspNetCore.Configure.Configure;
using System.Threading;
using Microsoft.AspNetCore.TestHost;
using System.Threading.Tasks;
using RTUITLab.AspNetCore.Configure.Invokations;
using RTUITLab.AspNetCore.Configure.Configure.Interfaces;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using RTUITLab.AspNetCore.Configure.Tests.TestWorks;

namespace RTUITLab.AspNetCore.Configure.Tests
{
    public class ConfigureLambdaBehaviorTests : BaseTests
    {
        public ConfigureLambdaBehaviorTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        private TestServer CreateWaitingSimpleServer(
            TimeSpan timeToWait,
            Action lockAction = null,
            Action continueAction = null)
            => new TestServer(CreateDefaultWebHostBuilder()
                .ConfigureServices(s => s
                    .AddSingleton(new WaitingWork(timeToWait))
                    .AddWebAppConfigure()
                        .SetBehavior(
                            lockAction: (c, n, s) => { lockAction?.Invoke(); return n(c); },
                            continueAction: (c, n) => { continueAction?.Invoke(); return n(c); })
                        .AddCongifure<WaitingWork>()));

        private TestServer CreateSimpleServerWithWork<T>(
            Action lockAction = null,
            Action continueAction = null) where T: class, IConfigureWork
            => new TestServer(CreateDefaultWebHostBuilder()
                .ConfigureServices(s => s
                    .AddSingleton<T>()
                    .AddWebAppConfigure()
                        .SetBehavior(
                            lockAction: (c, n, s) => { lockAction?.Invoke(); return n(c); },
                            continueAction: (c, n) => { continueAction?.Invoke(); return n(c); })
                        .AddCongifure<T>()));

        private TestServer CreateInfiniteLockSimpleServer(
            Action lockAction = null,
            Action continueAction = null) => CreateWaitingSimpleServer(Timeout.InfiniteTimeSpan, lockAction, continueAction);

        private TestServer CreateZeroLockSimpleServer(
            Action lockAction = null,
            Action continueAction = null) => CreateWaitingSimpleServer(TimeSpan.Zero, lockAction, continueAction);

        [Fact]
        public async Task Configure_LockPathShouldBeIbvoked()
        {
            var testValue = false;
            var server = CreateInfiniteLockSimpleServer(lockAction: () => testValue = true);
            var client = server.CreateClient();
            await client.GetAsync("");
            Assert.True(testValue, "test value must be changed after request by lock path behavior");
        }

        [Fact]
        public async Task Configure_ContinuePathShouldBeIbvoked()
        {
            var testValue = false;
            var server = CreateZeroLockSimpleServer(continueAction: () => testValue = true);
            var client = server.CreateClient();
            await client.GetAsync("");
            Assert.True(testValue, "test value must be changed after request by continue path behavior");
        }

        [Fact]
        public async Task Configure_ContinueActionShouldBeInvokedAfterLock()
        {
            var lockValue = false;
            var continueValue = false;

            var server = CreateSimpleServerWithWork<ManuallyEndWork>(
                lockAction: () => lockValue = true,
                continueAction: () => continueValue = true
            );

            var manuallyWork = server.Services.GetRequiredService<ManuallyEndWork>();

            var client = server.CreateClient();
            
            await client.GetAsync("lock");
            Assert.True(lockValue, "lock value must be changed after request by lock path behavior");
            Assert.False(continueValue, "continue value must be stay false after request by lock path behavior");

            lockValue = false;
            manuallyWork.DoneAction();
            await client.GetAsync("continue");
            Assert.False(lockValue, "lock value must be stay false after request by continue path behavior");
            Assert.True(continueValue, "continue value must be changed after request by continue path behavior");
        }
    }
}
