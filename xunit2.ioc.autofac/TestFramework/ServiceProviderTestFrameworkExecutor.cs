using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.Microsoft.DI.TestFramework
{
    public class ServiceProviderTestFrameworkExecutor : TestFrameworkExecutor<ServiceProviderTestMethodTestCase>
    {
        public ServiceProviderTestFrameworkExecutor(AssemblyName assemblyName, IServiceProvider container, ISourceInformationProvider sourceInformationProvider, IMessageSink diagnosticMessageSink)
            : base(assemblyName, sourceInformationProvider, diagnosticMessageSink)
        {
            _container = container;
            _testAssembly = new TestAssembly(AssemblyInfo);
        }

        protected override ITestFrameworkDiscoverer CreateDiscoverer()
        {
            return new ServiceProviderTestFrameworkDiscoverer(AssemblyInfo, SourceInformationProvider, DiagnosticMessageSink);
        }

        protected override async void RunTestCases(IEnumerable<ServiceProviderTestMethodTestCase> testCases, IMessageSink executionMessageSink, ITestFrameworkExecutionOptions executionOptions)
        {
            using (var assemblyRunner = new ServiceProviderTestAssemblyRunner(_container, _testAssembly, testCases, DiagnosticMessageSink, executionMessageSink, executionOptions))
            {
                await assemblyRunner.RunAsync();
            }
        }

        private readonly IServiceProvider _container;
        private readonly TestAssembly _testAssembly;
    }
}
