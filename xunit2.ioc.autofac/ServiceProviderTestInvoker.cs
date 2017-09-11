using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using Xunit.Extensions.Microsoft.DI.TestFramework;
using Xunit.Sdk;

namespace Xunit.Extensions.Microsoft.DI
{
    public class ServiceProviderTestInvoker : TestInvoker<ServiceProviderTestMethodTestCase>
    {
        public const string TestLifetimeScopeTag = "TestLifetime";

        public ServiceProviderTestInvoker(IServiceProvider container, ITest test, IMessageBus messageBus, Type testClass, object[] constructorArguments, MethodInfo testMethod, object[] testMethodArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource) 
            : base(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, aggregator, cancellationTokenSource)
        {
            _lifetimeScope = container.CreateScope();
            _testOutputHelper = _lifetimeScope.ServiceProvider.GetService<TestOutputHelper>();
        }

        public string Output { get; set; }

        protected override Task BeforeTestMethodInvokedAsync()
        {
            _testOutputHelper?.Initialize(MessageBus, Test);

            return base.BeforeTestMethodInvokedAsync();
        }

        protected override Task AfterTestMethodInvokedAsync()
        {
            if (_testOutputHelper != null)
            {
                Output = _testOutputHelper.Output;
                _testOutputHelper.Uninitialize();
            }

            _lifetimeScope?.Dispose();
            return base.AfterTestMethodInvokedAsync();
        }

        protected override object CreateTestClass()
        {
            return _lifetimeScope.ServiceProvider.GetService(TestClass);
        }

        private readonly IServiceScope _lifetimeScope;
        private readonly TestOutputHelper _testOutputHelper;
    }
}