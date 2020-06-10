using System.Collections.Generic;
using System.Net.Http;

namespace Mobile.WebApi
{
    public static class WebApiSettings
    {
        internal static Dictionary<string, string> DevUriRoutes = new Dictionary<string, string>
        {
            { "Accounts", "https://10.0.2.2:5005/Account" },
            { "Friendships", "https://10.0.2.2:5003/Friendship" },
            { "Messages", "https://10.0.2.2:5001/MessageSession" },
            { "Images", "https://10.0.2.2:5006/Image" }
        };

        internal static Dictionary<string, string> ServerUriRoutes = new Dictionary<string, string>
        {
            { "Accounts", "https://52.12.195.150:5005/Account" },
            { "Friendships", "https://52.12.195.150:5003/Friendship" },
            { "Messages", "https://52.12.195.150:5001/MessageSession" },
            { "Images", "https://52.12.195.150:5006/Image" }
        };
        public static HttpClientHandler CreateHandler()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) =>
            {
                if (cert.Issuer.Equals("CN=localhost"))
                    return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };

            return handler;
        }
    }
}
