using RTUITLab.AspNetCore.Configure.Behavior;
using RTUITLab.AspNetCore.Configure.Invokations;
using System;

namespace RTUITLab.AspNetCore.Configure.Shared.Interfaces
{
    public interface IWorkPathGetter
    {
        WorkHandlePath GetConfigureStatus(out ConfigurungStatus status);
    }
}
