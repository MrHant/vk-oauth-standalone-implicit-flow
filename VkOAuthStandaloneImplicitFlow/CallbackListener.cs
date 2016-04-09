using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace VkOAuthStandaloneImplicitFlow
{
    internal class CallbackListener
    {
        internal CallbackListener(int port)
        {
            _port = port;
            _listener = new HttpListener();
        }

        internal AuthDetails GetAuthResponse()
        {
            _listener.Prefixes.Add(ListenerUri);
            _listener.Start();

            ProcessVkRequest();
            var secondResponse = ProcessJsRequest();

            _listener.Stop();

            return new AuthDetails
            {
                AccessToken = secondResponse["access_token"],
                ExpiresInSec = int.Parse(secondResponse["expires_in"]),
                UserId = long.Parse(secondResponse["user_id"])
            };
        }

        private NameValueCollection ProcessVkRequest()
        {
            using (var reader = new StreamReader(TemplateResourceStream))
            {
                var responseBody = reader.ReadToEnd();
                return ProcessAndGetRequestUrl(responseBody);
            }
        }

        private NameValueCollection ProcessJsRequest()
        {
            return ProcessAndGetRequestUrl();
        }

        private NameValueCollection ProcessAndGetRequestUrl(string responseBody = "")
        {
            var context = _listener.GetContext();
            var request = context.Request;

            var response = context.Response;
            response.ContentType = "text/html; charset=UTF-8";
            var buffer = Encoding.UTF8.GetBytes(responseBody);
            response.ContentLength64 = buffer.Length;

            using (var output = response.OutputStream)
            {
                output.Write(buffer, 0, buffer.Length);
            }

            return request.QueryString;
        }

        internal string CallbackUrl => $"{ListenerUri}callback";

        private string ListenerUri => $"http://localhost:{_port}/";

        private static string TemplateName => "AuthCallback.html";


        private static Stream TemplateResourceStream
        {
            get
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var resourceName = $"{assembly.GetName().Name}.{TemplateName}";
                return assembly.GetManifestResourceStream(resourceName);
            }
        }

        private readonly int _port;
        private readonly HttpListener _listener;
    }
}

