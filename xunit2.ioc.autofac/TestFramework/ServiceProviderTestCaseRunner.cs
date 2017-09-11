using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace Xunit.Extensions.Microsoft.DI.TestFramework
{
    public class ServiceProviderTestCaseRunner : TestCaseRunner<ServiceProviderTestMethodTestCase>
    {
        public ServiceProviderTestCaseRunner(ServiceProviderTestMethodTestCase serviceProviderTestMethodTestCase, IServiceProvider container, IMessageBus messageBus, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, string displayName) 
            : base(serviceProviderTestMethodTestCase, messageBus, aggregator, cancellationTokenSource)
        {
            _container = container;
            _displayName = displayName;
        }

        protected override Task<RunSummary> RunTestAsync()
        {
            var testClass = TestCase.TestMethod.TestClass.Class.ToRuntimeType();
            var testMethod = TestCase.TestMethod.Method.ToRuntimeMethod();
            var test = new ServiceProviderTest(TestCase, _displayName);

            return new ServiceProviderTestRunner(_container, test, MessageBus, testClass, null, testMethod, null, "", Aggregator, CancellationTokenSource)
                .RunAsync();
        }

        private readonly IServiceProvider _container;
        private readonly string _displayName;
    }
}