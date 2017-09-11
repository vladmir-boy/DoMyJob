using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.Microsoft.DI.TestFramework
{
    public class ServiceProviderTestMethodRunner : TestMethodRunner<ServiceProviderTestMethodTestCase>
    {
        public ServiceProviderTestMethodRunner(IServiceProvider container, IMessageSink diagnosticMessageSink, ITestMethod testMethod, IReflectionTypeInfo @class, IReflectionMethodInfo method, IEnumerable<ServiceProviderTestMethodTestCase> testCases, IMessageBus messageBus, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource) 
            : base(testMethod, @class, method, testCases, messageBus, aggregator, cancellationTokenSource)
        {
            _container = container;
            _diagnosticMessageSink = diagnosticMessageSink;
        }

        protected override Task<RunSummary> RunTestCaseAsync(ServiceProviderTestMethodTestCase serviceProviderTestMethodTestCase)
        {
            return serviceProviderTestMethodTestCase.RunAsync(_container, _diagnosticMessageSink, MessageBus, Aggregator, CancellationTokenSource);
        }

        private readonly IServiceProvider _container;
        private readonly IMessageSink _diagnosticMessageSink;
    }
}