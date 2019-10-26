using Microsoft.Extensions.DependencyInjection;
using RTUITLab.AspNetCore.Configure.Configure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RTUITLab.AspNetCore.Configure.Invokations
{
    internal class WorkItem
    {
        public int Id { get; set; }
        public Task<int> WorkTask { get; set; }
        public IConfigurationWorkBuilder Builder { get; set; }
        public IConfigureWork Work { get; set; }
        public IServiceScope ServiceScope { get; set; }
    }
}
