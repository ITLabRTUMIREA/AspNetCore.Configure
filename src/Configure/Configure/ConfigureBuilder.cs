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
using Microsoft.Extensions.Hosting;
using System.Linq;

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
            serviceCollection.AddHostedService<ConfigureExecutorHostedService>();
            serviceCollection.AddSingleton<IWorkPathGetter>(sp => sp.GetServices<IHostedService>().Single(s => s is ConfigureExecutorHostedService) as ConfigureExecutorHostedService);
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

        /// <summary>
        /// Set behavior based on Funcs. Uses <see cref="DefaultBehavior"/> logic if behavior func is not present
        /// </summary>
        /// <param name="lockAction">Func that will be used on the lock path</param>
        /// <param name="continueAction">Func that will be used on the continue path</param>
        /// <returns></returns>
        public ConfigureBuilder SetBehavior(
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

        /// <summary>
        /// Gets behavior <typeparamref name="T"/> from DI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Updated builder</returns>
        public ConfigureBuilder SetBehavior<T>() where T : class, IBehavior
        {
            serviceCollection.AddTransient<T>();
            Behavior = new InDIBehavior<T>();
            return this;
        }

        /// <summary>
        /// Adds type <typeparamref name="T"/> to DI container as Transient and use it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Updated builder</returns>
        public ConfigureBuilder SetTransientBehavior<T>() where T : class, IBehavior
        {
            serviceCollection.AddTransient<T>();
            Behavior = new InDIBehavior<T>();
            return this;
        }

        /// <summary>
        /// Adds type <typeparamref name="T"/> to DI container as Singleton and use it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Updated builder</returns>
        public ConfigureBuilder SetSingletonBehavior<T>() where T : class, IBehavior
        {
            serviceCollection.AddSingleton<T>();
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
