using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using RTUITLab.AspNetCore.Configure.Invokations;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;

namespace RTUITLab.AspNetCore.Configure.Tests
{
    public class BaseTests
    {
        private readonly ITestOutputHelper outputHelper;

        public BaseTests(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper;
        }

        //private void CreateBasic(out )

        protected WebHostBuilder CreateDefaultWebHostBuilder()
        {
            var webHostBuilder = new WebHostBuilder();
            webHostBuilder.ConfigureLogging(lb => lb.AddXUnit(outputHelper).AddFilter("RTUITLab", LogLevel.Trace));
            webHostBuilder.Configure(app => app.UseWebAppConfigure());
            return webHostBuilder;
        }
    }
}
