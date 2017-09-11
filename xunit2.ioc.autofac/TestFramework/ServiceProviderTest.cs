using Xunit.Abstractions;

namespace Xunit.Extensions.Microsoft.DI.TestFramework
{
    public class ServiceProviderTest : LongLivedMarshalByRefObject, ITest
    {
        public ServiceProviderTest(ServiceProviderTestMethodTestCase serviceProviderTestMethodTestCase, string displayName)
        {
            ServiceProviderTestMethodTestCase = serviceProviderTestMethodTestCase;
            DisplayName = displayName;
        }

        public string DisplayName { get; }
        public ServiceProviderTestMethodTestCase ServiceProviderTestMethodTestCase { get; }

        ITestCase ITest.TestCase => ServiceProviderTestMethodTestCase;
    }
}