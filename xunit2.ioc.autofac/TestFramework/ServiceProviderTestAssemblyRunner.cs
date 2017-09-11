using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.Microsoft.DI.TestFramework
{
    public class ServiceProviderTestAssemblyRunner : TestAssemblyRunner<ServiceProviderTestMethodTestCase>
    {
        public ServiceProviderTestAssemblyRunner(IServiceProvider container, ITestAssembly testAssembly, IEnumerable<ServiceProviderTestMethodTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageSink executionMessageSink,
            ITestFrameworkExecutionOptions executionOptions)
            : base(testAssembly, testCases, diagnosticMessageSink, executionMessageSink, executionOptions)
        {
            _container = container;
        }

        protected override string GetTestFrameworkDisplayName()
        {
            return "Microsoft.ServiceProvider Test Framework";
        }

        protected override Task<RunSummary> RunTestCollectionAsync(IMessageBus messageBus, ITestCollection testCollection, IEnumerable<ServiceProviderTestMethodTestCase> testCases, CancellationTokenSource cancellationTokenSource)
        {
            return new ServiceProviderTestCollectionRunner(_container, testCollection, testCases, DiagnosticMessageSink, messageBus, TestCaseOrderer, new ExceptionAggregator(Aggregator), cancellationTokenSource)
                .RunAsync();
        }

        private readonly IServiceProvider _container;
    }
}