using System.Net.Http;

namespace Mobile.WebApi
{
    public static class WebApiSettings
    {
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
