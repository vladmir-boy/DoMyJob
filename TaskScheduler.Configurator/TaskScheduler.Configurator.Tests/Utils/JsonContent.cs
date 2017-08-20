using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace TaskScheduler.Configurator.Tests.Utils
{
    public static class JsonContent
    {
        public static StringContent Create<T>(T obj)
        {
            return new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
        }
    }
}