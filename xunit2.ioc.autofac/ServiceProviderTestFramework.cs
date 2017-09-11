using System;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Extensions.Microsoft.DI.TestFramework;
using Xunit.Sdk;

namespace Xunit.Extensions.Microsoft.DI
{
    public class ServiceProviderTestFramework : XunitTestFramework
    {
        protected ServiceProviderTestFramework(IMessageSink diagnosticMessageSink)
            : base(diagnosticMessageSink)
        {
        }

        protected override ITestFrameworkDiscoverer CreateDiscoverer(IAssemblyInfo assemblyInfo) 
            => new ServiceProviderTestFrameworkDiscoverer(assemblyInfo, SourceInformationProvider, DiagnosticMessageSink);

        protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
            => new ServiceProviderTestFrameworkExecutor(assemblyName, Container, SourceInformationProvider, DiagnosticMessageSink);

        protected IServiceProvider Container;
    }
}
