using System;
using System.Threading;
using System.Threading.Tasks;

namespace RTUITLab.AspNetCore.Configure.Configure.Interfaces
{
    public interface IConfigureWork
    {
        Task Configure(CancellationToken cancellationToken);
    }
}
