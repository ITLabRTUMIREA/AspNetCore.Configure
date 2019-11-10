using RTUITLab.AspNetCore.Configure.Behavior;
using RTUITLab.AspNetCore.Configure.Configure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RTUITLab.AspNetCore.Configure.Configure
{
    class ConfigurationCase : IConfigurationCase
    {
        public ConfigurationCase(Type configureWorkType, WorkHandlePath workHandlePath, int priority)
        {
            ConfigureWorkType = configureWorkType ?? throw new ArgumentNullException(nameof(configureWorkType));
            WorkHandlePath = workHandlePath;
            Priority = priority;
        }
        public Type ConfigureWorkType { get; }

        public WorkHandlePath WorkHandlePath { get; }

        public int Priority { get; }
    }
}
