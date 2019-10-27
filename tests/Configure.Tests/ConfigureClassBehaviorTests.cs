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

        private TestServer CreateWaitingSimpleServerDIBehavior<T>(TimeSpan timeToWait) where T : class, IBehavior
            => new TestServer(CreateDefaultWebHostBuilder()
                .ConfigureServices(s => s
                    .AddSingleton(new WaitingWork(timeToWait))
                    .AddSingleton<T>()
                    .AddWebAppConfigure()
                        .SetBehavior<T>()
                        .AddCongifure<WaitingWork>()));

        private TestServer CreateWaitingSimpleServerSingletonBehavior<T> (TimeSpan timeToWait) where T : class, IBehavior
            => new TestServer(CreateDefaultWebHostBuilder()
                .ConfigureServices(s => s
                    .AddSingleton(new WaitingWork(timeToWait))
                    .AddWebAppConfigure()
                        .SetSingletonBehavior<T>()
                        .AddCongifure<WaitingWork>()));

        private TestServer CreateWaitingSimpleServerTransientBehavior<T>(TimeSpan timeToWait) where T : class, IBehavior
            => new TestServer(CreateDefaultWebHostBuilder()
                .ConfigureServices(s => s
                    .AddSingleton(new WaitingWork(timeToWait))
                    .AddWebAppConfigure()
                        .SetTransientBehavior<T>()
                        .AddCongifure<WaitingWork>()));

        private TestServer CreateSimpleServerWithWorkDIBehavior<TWork, TBehavior>()
                where TWork : class, IConfigureWork
                where TBehavior : class, IBehavior
            => new TestServer(CreateDefaultWebHostBuilder()
                .ConfigureServices(s => s
                    .AddSingleton<TWork>()
                    .AddSingleton<TBehavior>()
                    .AddWebAppConfigure()
                        .SetBehavior<TBehavior>()
                        .AddCongifure<TWork>()));

        private TestServer CreateSimpleServerWithWorkSingletonBehavior<TWork, TBehavior>() 
                where TWork: class, IConfigureWork
                where TBehavior: class, IBehavior
            => new TestServer(CreateDefaultWebHostBuilder()
                .ConfigureServices(s => s
                    .AddSingleton<TWork>()
                    .AddWebAppConfigure()
                        .SetSingletonBehavior<TBehavior>()
                        .AddCongifure<TWork>()));

        private TestServer CreateSimpleServerWithWorkTransientBehavior<TWork, TBehavior>()
                where TWork : class, IConfigureWork
                where TBehavior : class, IBehavior
            => new TestServer(CreateDefaultWebHostBuilder()
                .ConfigureServices(s => s
                    .AddSingleton<TWork>()
                    .AddWebAppConfigure()
                        .SetTransientBehavior<TBehavior>()
                        .AddCongifure<TWork>()));

        private TestServer CreateInfiniteLockSimpleServerDIBehavior<T>() where T : class, IBehavior
            => CreateWaitingSimpleServerDIBehavior<T>(Timeout.InfiniteTimeSpan);

        private TestServer CreateInfiniteLockSimpleServerSingletonBehavior<T>() where T : class, IBehavior
            => CreateWaitingSimpleServerSingletonBehavior<T>(Timeout.InfiniteTimeSpan);

        private TestServer CreateInfiniteLockSimpleServerTransientBehavior<T>() where T : class, IBehavior
            => CreateWaitingSimpleServerTransientBehavior<T>(Timeout.InfiniteTimeSpan);


        private TestServer CreateZeroLockSimpleServerDIBehavior<T>() where T : class, IBehavior
            => CreateWaitingSimpleServerDIBehavior<T>(TimeSpan.Zero);

        private TestServer CreateZeroLockSimpleServerSingletonBehavior<T>() where T : class, IBehavior
            => CreateWaitingSimpleServerSingletonBehavior<T>(TimeSpan.Zero);

        private TestServer CreateZeroLockSimpleServerTransientBehavior<T>() where T : class, IBehavior
            => CreateWaitingSimpleServerTransientBehavior<T>(TimeSpan.Zero);

        [Fact]
        public async Task Configure_LockPathShouldBeIbvoked_DI()
        {
            var server = CreateInfiniteLockSimpleServerDIBehavior<BoolValuesBehavior>();
            var client = server.CreateClient();
            await client.GetAsync("");
            var valuesStorage = server.Services.GetService<TestValuesStorage>();
            Assert.NotNull(valuesStorage);
            Assert.True(valuesStorage.LockValue, $"{nameof(valuesStorage.LockValue)} value must be changed after request by lock path behavior");
        }

        [Fact]
        public async Task Configure_LockPathShouldBeIbvoked_Singleton()
        {
            var server = CreateInfiniteLockSimpleServerSingletonBehavior<BoolValuesBehavior>();
            var client = server.CreateClient();
            await client.GetAsync("");
            var valuesStorage = server.Services.GetService<TestValuesStorage>();
            Assert.NotNull(valuesStorage);
            Assert.True(valuesStorage.LockValue, $"{nameof(valuesStorage.LockValue)} value must be changed after request by lock path behavior");
        }

        [Fact]
        public async Task Configure_LockPathShouldBeIbvoked_Transient()
        {
            var server = CreateInfiniteLockSimpleServerTransientBehavior<BoolValuesBehavior>();
            var client = server.CreateClient();
            await client.GetAsync("");
            var valuesStorage = server.Services.GetService<TestValuesStorage>();
            Assert.NotNull(valuesStorage);
            Assert.True(valuesStorage.LockValue, $"{nameof(valuesStorage.LockValue)} value must be changed after request by lock path behavior");
        }

        [Fact]
        public async Task Configure_ContinuePathShouldBeIbvoked_DI()
        {
            var server = CreateZeroLockSimpleServerDIBehavior<BoolValuesBehavior>();
            var client = server.CreateClient();
            await client.GetAsync("");
            var valueStorage = server.Services.GetService<TestValuesStorage>();
            Assert.NotNull(valueStorage);
            Assert.True(valueStorage.ContinueValue, $"{nameof(valueStorage.ContinueValue)} value must be changed after request by continue path behavior");
        }

        [Fact]
        public async Task Configure_ContinuePathShouldBeIbvoked_Singleton()
        {
            var server = CreateZeroLockSimpleServerSingletonBehavior<BoolValuesBehavior>();
            var client = server.CreateClient();
            await client.GetAsync("");
            var valueStorage = server.Services.GetService<TestValuesStorage>();
            Assert.NotNull(valueStorage);
            Assert.True(valueStorage.ContinueValue, $"{nameof(valueStorage.ContinueValue)} value must be changed after request by continue path behavior");
        }

        [Fact]
        public async Task Configure_ContinuePathShouldBeIbvoked_Transient()
        {
            var server = CreateZeroLockSimpleServerTransientBehavior<BoolValuesBehavior>();
            var client = server.CreateClient();
            await client.GetAsync("");
            var valueStorage = server.Services.GetService<TestValuesStorage>();
            Assert.NotNull(valueStorage);
            Assert.True(valueStorage.ContinueValue, $"{nameof(valueStorage.ContinueValue)} value must be changed after request by continue path behavior");
        }

        [Fact]
        public async Task Configure_ContinueActionShouldBeInvokedAfterLock_DI()
        {
            var server = CreateSimpleServerWithWorkDIBehavior<ManuallyEndWork, BoolValuesBehavior>();

            var manuallyWork = server.Services.GetRequiredService<ManuallyEndWork>();

            var client = server.CreateClient();
            var testValueStorage = server.Services.GetService<TestValuesStorage>();
            Assert.NotNull(testValueStorage);

            await client.GetAsync("lock");
            Assert.True(testValueStorage.LockValue, $"{nameof(testValueStorage.LockValue)} value must be changed after request by lock path behavior");
            Assert.False(testValueStorage.ContinueValue, $"{nameof(testValueStorage.ContinueValue)} value must be stay false after request by lock path behavior");

            testValueStorage.LockValue = false;
            manuallyWork.DoneAction();
            await client.GetAsync("continue");
            Assert.False(testValueStorage.LockValue, $"{nameof(testValueStorage.LockValue)} value must be stay false after request by continue path behavior");
            Assert.True(testValueStorage.ContinueValue, $"{nameof(testValueStorage.ContinueValue)} value must be changed after request by continue path behavior");
        }

        [Fact]
        public async Task Configure_ContinueActionShouldBeInvokedAfterLock_Singleton()
        {
            var server = CreateSimpleServerWithWorkSingletonBehavior<ManuallyEndWork, BoolValuesBehavior>();

            var manuallyWork = server.Services.GetRequiredService<ManuallyEndWork>();

            var client = server.CreateClient();
            var testValueStorage = server.Services.GetService<TestValuesStorage>();
            Assert.NotNull(testValueStorage);

            await client.GetAsync("lock");
            Assert.True(testValueStorage.LockValue, $"{nameof(testValueStorage.LockValue)} value must be changed after request by lock path behavior");
            Assert.False(testValueStorage.ContinueValue, $"{nameof(testValueStorage.ContinueValue)} value must be stay false after request by lock path behavior");

            testValueStorage.LockValue = false;
            manuallyWork.DoneAction();
            await client.GetAsync("continue");
            Assert.False(testValueStorage.LockValue, $"{nameof(testValueStorage.LockValue)} value must be stay false after request by continue path behavior");
            Assert.True(testValueStorage.ContinueValue, $"{nameof(testValueStorage.ContinueValue)} value must be changed after request by continue path behavior");
        }

        [Fact]
        public async Task Configure_ContinueActionShouldBeInvokedAfterLock_Transient()
        {
            var server = CreateSimpleServerWithWorkTransientBehavior<ManuallyEndWork, BoolValuesBehavior>();

            var manuallyWork = server.Services.GetRequiredService<ManuallyEndWork>();

            var client = server.CreateClient();
            var testValueStorage = server.Services.GetService<TestValuesStorage>();
            Assert.NotNull(testValueStorage);

            await client.GetAsync("lock");
            Assert.True(testValueStorage.LockValue, $"{nameof(testValueStorage.LockValue)} value must be changed after request by lock path behavior");
            Assert.False(testValueStorage.ContinueValue, $"{nameof(testValueStorage.ContinueValue)} value must be stay false after request by lock path behavior");

            testValueStorage.LockValue = false;
            manuallyWork.DoneAction();
            await client.GetAsync("continue");
            Assert.False(testValueStorage.LockValue, $"{nameof(testValueStorage.LockValue)} value must be stay false after request by continue path behavior");
            Assert.True(testValueStorage.ContinueValue, $"{nameof(testValueStorage.ContinueValue)} value must be changed after request by continue path behavior");
        }
    }
}
