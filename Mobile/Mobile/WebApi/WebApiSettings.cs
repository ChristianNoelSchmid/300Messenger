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
            { "Accounts", "http://52.12.195.150:80/Accounts" },
            { "Friendships", "https://52.12.195.150:80/Friendships" },
            { "Messages", "https://52.12.195.150:80/Messages" },
            { "Images", "https://52.12.195.150:80/Images" }
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
