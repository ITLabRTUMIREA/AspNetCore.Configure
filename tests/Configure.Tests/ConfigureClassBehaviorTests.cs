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
using RTUITLab.AspNetCore.Configure.Behavior.Interfaces;
using RTUITLab.AspNetCore.Configure.Tests.TestBehaviors;

namespace RTUITLab.AspNetCore.Configure.Tests
{
    public class ConfigureClassBehaviorTests : BaseTests
    {
        public ConfigureClassBehaviorTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        private TestServer CreateWaitingSimpleServerSingletonBehavior<T> (TimeSpan timeToWait) where T : class, IBehavior
            => new TestServer(CreateDefaultWebHostBuilder()
                .ConfigureServices(s => s
                    .AddSingleton(new WaitingWork(timeToWait))
                    .AddWebAppConfigure()
                        .SetSingletonBehavior<T>()
                        .AddCongifure<WaitingWork>()));

        private TestServer CreateSimpleServerWithWorkSingletonBehavior<TWork, TBehavior>(
            Action lockAction = null,
            Action continueAction = null) 
                where TWork: class, IConfigureWork
                where TBehavior: class, IBehavior
            => new TestServer(CreateDefaultWebHostBuilder()
                .ConfigureServices(s => s
                    .AddSingleton<TWork>()
                    .AddWebAppConfigure()
                        .SetSingletonBehavior<TBehavior>()
                        .AddCongifure<TWork>()));

        private TestServer CreateInfiniteLockSimpleServer<T>() where T : class, IBehavior
            => CreateWaitingSimpleServerSingletonBehavior<T>(Timeout.InfiniteTimeSpan);

        private TestServer CreateZeroLockSimpleServer<T>() where T : class, IBehavior
            => CreateWaitingSimpleServerSingletonBehavior<T>(TimeSpan.Zero);

        [Fact]
        public async Task Configure_LockPathShouldBeIbvoked()
        {
            var server = CreateInfiniteLockSimpleServer<BoolValuesBehavior>();
            var client = server.CreateClient();
            await client.GetAsync("");
            var boolValuesBehavior = server.Services.GetService<BoolValuesBehavior>();
            Assert.NotNull(boolValuesBehavior);
            Assert.True(boolValuesBehavior.LockValue, $"{nameof(boolValuesBehavior.LockValue)} value must be changed after request by lock path behavior");
        }

        [Fact]
        public async Task Configure_ContinuePathShouldBeIbvoked()
        {
            var server = CreateZeroLockSimpleServer<BoolValuesBehavior>();
            var client = server.CreateClient();
            await client.GetAsync("");
            var boolValuesBehavior = server.Services.GetService<BoolValuesBehavior>();
            Assert.NotNull(boolValuesBehavior);
            Assert.True(boolValuesBehavior.ContinueValue, $"{nameof(boolValuesBehavior.ContinueValue)} value must be changed after request by continue path behavior");
        }

        [Fact]
        public async Task Configure_ContinueActionShouldBeInvokedAfterLock()
        {
            var server = CreateSimpleServerWithWorkSingletonBehavior<ManuallyEndWork, BoolValuesBehavior>();

            var manuallyWork = server.Services.GetRequiredService<ManuallyEndWork>();

            var client = server.CreateClient();
            var boolValuesBehavior = server.Services.GetService<BoolValuesBehavior>();
            Assert.NotNull(boolValuesBehavior);

            await client.GetAsync("lock");
            Assert.True(boolValuesBehavior.LockValue, $"{nameof(boolValuesBehavior.LockValue)} value must be changed after request by lock path behavior");
            Assert.False(boolValuesBehavior.ContinueValue, $"{nameof(boolValuesBehavior.ContinueValue)} value must be stay false after request by lock path behavior");

            boolValuesBehavior.LockValue = false;
            manuallyWork.DoneAction();
            await client.GetAsync("continue");
            Assert.False(boolValuesBehavior.LockValue, $"{nameof(boolValuesBehavior.LockValue)} value must be stay false after request by continue path behavior");
            Assert.True(boolValuesBehavior.ContinueValue, $"{nameof(boolValuesBehavior.ContinueValue)} value must be changed after request by continue path behavior");
        }
    }
}
