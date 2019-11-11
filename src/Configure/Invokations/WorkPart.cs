using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTUITLab.AspNetCore.Configure.Invokations
{
    internal class WorkPart
    {
        public int Priority { get; set; }
        public List<WorkItem> WorkItems { get; set; }
        public TaskStatus Status => WorkItems.Select(wi => wi.Status).Max();
    }
}
