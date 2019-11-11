using System;
using System.Collections.Generic;
using System.Text;

namespace RTUITLab.AspNetCore.Configure.Configure.Interfaces
{
    public interface IConfigurationCaseStorage
    {
        IEnumerable<IConfigurationCase> Works { get; }
    }
}
