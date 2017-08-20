using System.Net.Http;
using System.Text.RegularExpressions;

namespace TaskScheduler.Configurator.Tests.Utils
{
    public class UrlParser
    {
        internal static int ExtractIdFromLocationHeader(HttpResponseMessage responseMessage, string apiPath)
        {
            var regex = new Regex($@"http[s]*:\/\/[a-z0-9:]+{apiPath}\/([0-9])", RegexOptions.IgnoreCase);
            var match = regex.Match(responseMessage.Headers.Location.AbsoluteUri);
            return int.Parse(match.Groups[1].Value);
        }
    }
}