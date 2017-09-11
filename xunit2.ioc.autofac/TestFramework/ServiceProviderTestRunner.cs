using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.Microsoft.DI.TestFramework
{
    public class ServiceProviderTestRunner : TestRunner<ServiceProviderTestMethodTestCase>
    {
        public ServiceProviderTestRunner(IServiceProvider container, ITest test, IMessageBus messageBus, Type testClass, object[] constructorArguments, MethodInfo testMethod, object[] testMethodArguments, string skipReason, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource) 
            : base(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, skipReason, aggregator, cancellationTokenSource)
        {
            _container = container;
        }

        protected override async Task<Tuple<decimal, string>> InvokeTestAsync(ExceptionAggregator aggregator)
        {
            var invoker = new ServiceProviderTestInvoker(_container, Test, MessageBus, TestClass, null, TestMethod, null, aggregator, CancellationTokenSource);
            var duration = await invoker.RunAsync();

            return Tuple.Create(duration, invoker.Output);
        }

        private readonly IServiceProvider _container;
    }
}