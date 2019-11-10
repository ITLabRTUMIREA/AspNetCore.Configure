using System;
using System.Collections.Generic;
using System.Text;

namespace RTUITLab.AspNetCore.Configure.Invokations
{
    internal class WorkPart
    {
        public int Priority { get; set; }
        public List<WorkItem> WorkItems { get; set; }
    }
}
