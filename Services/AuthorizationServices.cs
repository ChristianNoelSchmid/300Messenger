using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Services
{
    public static class AuthorizationServices
    {
        private static string SECRET = "a_d3rpy_f1uffy_c0rg1";
        private static HttpClient verifyClient = null;

        public static async Task<string> VerifyToken(IHttpClientFactory clientFactory, string token)
        {
            // Used for testing - if the secret is given, just return the token with
            // the secret removed (should be the email address)
            if (token.StartsWith(SECRET)) return token.Replace(SECRET, "");

            if (verifyClient == null)
            {
                var handler = new HttpClientHandler();
                var verifyClient = new HttpClient(handler);

                // Retrieve the Environment Variable for ASP.NET Core's 
                // Environment mode
                // If it's under 'Development', allow all SSL connections to
                // avoid connections issues.
                var mode = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (mode == "Development")
                {
                    handler.ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                }

                var verifyURI = Environment.GetEnvironmentVariable("JWTTOKEN_VERIFICATION_URI");
                verifyClient.BaseAddress = new Uri(verifyURI);
            }

            verifyClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", token);

            var response = await verifyClient.GetAsync("");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return null;
        }
    }
}
