using System;
using Microsoft.Extensions.DependencyInjection;
using RTUITLab.AspNetCore.Configure.Behavior;
using RTUITLab.AspNetCore.Configure.Configure.Interfaces;

namespace RTUITLab.AspNetCore.Configure.Configure
{
    public class ConfigureWorkBuilder<T> where T : class, IConfigureWork
    {
        private readonly IServiceCollection serviceCollection;

        private WorkHandlePath workHandlePath = WorkHandlePath.Lock;
        private int priority = 0;

        public ConfigureWorkBuilder(IServiceCollection serviceCollection)
        {
            this.serviceCollection = serviceCollection;
        }

        public ConfigureWorkBuilder<T> LockWhileConfigure()
            => UseHandlePath(WorkHandlePath.Lock);


        public ConfigureWorkBuilder<T> ContinueWhileConfigure()
            => UseHandlePath(WorkHandlePath.Continue);

        public ConfigureWorkBuilder<T> UseHandlePath(WorkHandlePath handlePath)
        {
            workHandlePath = handlePath;
            return this;
        }

        public ConfigureWorkBuilder<T> SetPriority(int priority)
        {
            this.priority = priority;
            return this;
        }

        public ConfigureWorkBuilder<T> TransientImplementation<V>() where V : T
            => TransientImplementation(typeof(V));


        public ConfigureWorkBuilder<T> TransientImplementation(Type implementationType)
        {
            serviceCollection.AddTransient(typeof(T), implementationType);
            return this;
        }

        internal IConfigurationCase Build()
            => new ConfigurationCase(typeof(T), workHandlePath, priority);
    }
}
