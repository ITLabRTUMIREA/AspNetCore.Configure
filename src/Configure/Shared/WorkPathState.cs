using RTUITLab.AspNetCore.Configure.Behavior;
using RTUITLab.AspNetCore.Configure.Shared.Interfaces;

namespace RTUITLab.AspNetCore.Configure.Shared
{
    public class WorkPathState : IWorkPathGetter
    {
        public WorkHandlePath WorkHandlePath { get; set; }

        public WorkHandlePath GetHandlePath()
            => WorkHandlePath;

        public void SetHandlePath(WorkHandlePath path)
            => WorkHandlePath = path;
    }
}
