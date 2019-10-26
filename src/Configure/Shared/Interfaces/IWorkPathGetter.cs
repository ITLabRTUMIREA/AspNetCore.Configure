using RTUITLab.AspNetCore.Configure.Behavior;
using System;

namespace RTUITLab.AspNetCore.Configure.Shared.Interfaces
{
    public interface IWorkPathGetter
    {
        WorkHandlePath GetHandlePath();
        void SetHandlePath(WorkHandlePath path);
    }
}
