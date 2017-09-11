using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.Microsoft.DI.TestFramework
{
    public class ServiceProviderTestCollectionRunner : TestCollectionRunner<ServiceProviderTestMethodTestCase>
    {
        public ServiceProviderTestCollectionRunner(IServiceProvider container, ITestCollection testCollection, IEnumerable<ServiceProviderTestMethodTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageBus messageBus,
            ITestCaseOrderer testCaseOrderer, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
            : base(testCollection, testCases, messageBus, testCaseOrderer, aggregator, cancellationTokenSource)
        {
            _container = container;
            _diagnosticMessageSink = diagnosticMessageSink;
        }

        protected override Task<RunSummary> RunTestClassAsync(ITestClass testClass, IReflectionTypeInfo reflectionTypeInfo, IEnumerable<ServiceProviderTestMethodTestCase> testCases)
        {
            var exceptionAggregator = new ExceptionAggregator(Aggregator);

            var autofacTestClassRunner = new ServiceProviderTestClassRunner(_container, testClass, reflectionTypeInfo, testCases, 
                _diagnosticMessageSink, MessageBus, TestCaseOrderer, exceptionAggregator, 
                CancellationTokenSource);

            return autofacTestClassRunner.RunAsync();
        }

        private readonly IServiceProvider _container;
        private readonly IMessageSink _diagnosticMessageSink;
    }
}