using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.Microsoft.DI.TestFramework
{
    public class ServiceProviderTestClassRunner : TestClassRunner<ServiceProviderTestMethodTestCase>
    {
        private readonly IServiceProvider _container;

        public ServiceProviderTestClassRunner(IServiceProvider container, ITestClass testClass, IReflectionTypeInfo @class, IEnumerable<ServiceProviderTestMethodTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ITestCaseOrderer testCaseOrderer, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
            : base(testClass, @class, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource)
        {
            _container = container;
        }

        protected override Task<RunSummary> RunTestMethodAsync(ITestMethod testMethod, IReflectionMethodInfo method, IEnumerable<ServiceProviderTestMethodTestCase> testCases, object[] constructorArguments)
        {
            return new ServiceProviderTestMethodRunner(_container, DiagnosticMessageSink, testMethod, Class, method, testCases, MessageBus, new ExceptionAggregator(Aggregator), CancellationTokenSource)
                .RunAsync();
        }
    }
}
