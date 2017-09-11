using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.Microsoft.DI.TestFramework
{
    public class ServiceProviderTestMethodTestCase : TestMethodTestCase
    {
        public ServiceProviderTestMethodTestCase() { }

        public ServiceProviderTestMethodTestCase(TestMethodDisplay defaultMethodDisplay, ITestMethod testMethod)
            : base(defaultMethodDisplay, testMethod) { }

        public Task<RunSummary> RunAsync(IServiceProvider container, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
        {
            return new ServiceProviderTestCaseRunner(this, container, messageBus, aggregator, cancellationTokenSource, DisplayName)
                .RunAsync();
        }
    }
}