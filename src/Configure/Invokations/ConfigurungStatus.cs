using RTUITLab.AspNetCore.Configure.Behavior;
using System;
using System.Collections.Generic;
using System.Text;

namespace RTUITLab.AspNetCore.Configure.Invokations
{
    public class ConfigurungStatus
    {
        public IEnumerable<int> AllPriority { get; }
        public IEnumerable<int> DonePriority { get; }
        public ConfigurungStatus(IEnumerable<int> allPriority, IEnumerable<int> donePriority)
        {
            AllPriority = allPriority;
            DonePriority = donePriority;
        }
    }
}
