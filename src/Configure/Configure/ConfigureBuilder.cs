using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using RTUITLab.AspNetCore.Configure.Behavior.Interfaces;
using RTUITLab.AspNetCore.Configure.Behavior;
using RTUITLab.AspNetCore.Configure.Configure.Interfaces;
using RTUITLab.AspNetCore.Configure.Invokations;
using RTUITLab.AspNetCore.Configure.Shared.Interfaces;
using RTUITLab.AspNetCore.Configure.Shared;

namespace RTUITLab.AspNetCore.Configure.Configure
{
    public class ConfigureBuilder
    {
        private readonly List<IConfigurationWorkBuilder> builders
            = new List<IConfigurationWorkBuilder>();

        private readonly IServiceCollection serviceCollection;


        public IEnumerable<IConfigurationWorkBuilder> Builders => builders;
        public IBehavior Behavior { get; private set; } = new DefaultBehavior();


        public ConfigureBuilder(IServiceCollection serviceCollection)
        {
            this.serviceCollection = serviceCollection;
            serviceCollection.AddSingleton(this);
            serviceCollection.AddSingleton<IWorkPathGetter, WorkPathState>();
            serviceCollection.AddHostedService<ConfigureExecutorHostedService>();
        }

        public ConfigureBuilder AddTransientConfigure<T>(bool condition)
            where T : class, IConfigureWork
            => condition ? AddTransientConfigure<T>() : this;

        public ConfigureBuilder AddTransientConfigure<T>()
            where T : class, IConfigureWork
            => AddCongifure<T>(options => options.TransientImplementation<T>());

        public ConfigureBuilder AddTransientConfigure<T, V>()
            where T : class, IConfigureWork
            where V : T
            => AddCongifure<T>(options => options.TransientImplementation<V>());

        public ConfigureBuilder AddCongifure<T>(Action<ConfigureWorkBuilder<T>> configure = null) where T : class, IConfigureWork
        {
            var builder = new ConfigureWorkBuilder<T>(this, serviceCollection);
            configure?.Invoke(builder);
            builders.Add(builder);
            return this;
        }

        public ConfigureBuilder AddBehavior(
            Func<HttpContext, RequestDelegate, Task> lockAction = null,
            Func<HttpContext, RequestDelegate, Task> continueAction = null)
        {
            Behavior = new ConfiguredBehavior
            {
                ContinueAction = continueAction,
                LockAction = lockAction
            };
            return this;
        }
        public ConfigureBuilder AddBehavior<T>() where T : class, IBehavior
        {
            serviceCollection.AddTransient<T>();
            Behavior = new InDIBehavior<T>();
            return this;
        }

    }
    public static class WebAppConfigureBuilderExtensions
    {
        public static ConfigureBuilder AddWebAppConfigure(this IServiceCollection services)
        {
            return new ConfigureBuilder(services);
        }
    }

}
