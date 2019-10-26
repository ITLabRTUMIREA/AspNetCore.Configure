using System;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography.X509Certificates;
using RTUITLab.AspNetCore.Configure.Behavior;

namespace RTUITLab.AspNetCore.Configure.Configure.Interfaces
{
    public interface IConfigurationWorkBuilder
    {
        Type ConfigureWorkType { get; }
        WorkHandlePath WorkHandlePath { get; }
    }
}
