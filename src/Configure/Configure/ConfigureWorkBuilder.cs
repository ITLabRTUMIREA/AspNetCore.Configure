using System;
using Microsoft.Extensions.DependencyInjection;
using RTUITLab.AspNetCore.Configure.Behavior;
using RTUITLab.AspNetCore.Configure.Configure.Interfaces;

namespace RTUITLab.AspNetCore.Configure.Configure
{
    public class ConfigureWorkBuilder<T> : IConfigurationWorkBuilder
        where T : class, IConfigureWork
    {
        private ConfigureBuilder configureBuilder;
        private readonly IServiceCollection serviceCollection;

        public Type ConfigureWorkType => typeof(T);

        public WorkHandlePath WorkHandlePath { get; private set; } = WorkHandlePath.Lock;

        public ConfigureWorkBuilder(ConfigureBuilder configureBuilder, IServiceCollection serviceCollection)
        {
            this.configureBuilder = configureBuilder;
            this.serviceCollection = serviceCollection;
        }

        public ConfigureWorkBuilder<T> LockWhileConfigure()
            => UseHandlePath(WorkHandlePath.Lock);


        public ConfigureWorkBuilder<T> ContinueWhileConfigure()
            => UseHandlePath(WorkHandlePath.Continue);

        public ConfigureWorkBuilder<T> UseHandlePath(WorkHandlePath handlePath)
        {
            WorkHandlePath = handlePath;
            return this;
        }
        public ConfigureWorkBuilder<T> TransientImplementation<V>() where V : T
            => TransientImplementation(typeof(V));


        public ConfigureWorkBuilder<T> TransientImplementation(Type implementationType)
        {
            serviceCollection.AddTransient(typeof(T), implementationType);
            return this;
        }

        public ConfigureBuilder Done()
            => configureBuilder;
    }
}
