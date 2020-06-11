using System.Collections.Generic;
using System.Net.Http;

namespace Mobile.WebApi
{
    public static class WebApiSettings
    {
        internal static Dictionary<string, string> DevUriRoutes = new Dictionary<string, string>
        {
            { "Accounts", "https://10.0.2.2:5004/" },
            { "Friendships", "https://10.0.2.2:5005/" },
            { "Messages", "https://10.0.2.2:5006/" },
            { "Images", "https://10.0.2.2:5007/" }
        };

        internal static Dictionary<string, string> ServerUriRoutes = new Dictionary<string, string>
        {
            { "Accounts", "http://54.188.138.52/Accounts" },
            { "Friendships", "http://54.188.138.52/Friendships" },
            { "Messages", "http://54.188.138.52/Messages" },
            { "Images", "http://54.188.138.52/Images" }
        };
        public static HttpClientHandler CreateHandler()
        {
            HttpClientHandler handler = new HttpClientHandler();
            /*handler.ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) =>
            {
                if (cert.Issuer.Equals("CN=localhost"))
                    return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };*/

            return handler;
        }
    }
}
