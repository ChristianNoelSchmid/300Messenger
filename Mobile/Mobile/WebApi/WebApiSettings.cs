﻿using System.Collections.Generic;
using System.Net.Http;

namespace Mobile.WebApi
{
    public static class WebApiSettings
    {
        internal static Dictionary<string, string> DevUriRoutes = new Dictionary<string, string>
        {
            { "Accounts", "http://10.0.2.2:5000" },
            { "Friendships", "http://10.0.2.2:5001" },
            { "Messages", "http://10.0.2.2:5002" },
            { "Images", "http://10.0.2.2:5003" }
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
