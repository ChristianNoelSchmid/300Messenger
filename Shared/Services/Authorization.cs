using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace _300Messenger.Shared.Services
{
    /// <summary>
    /// Class which uses the Accounts microservice
    /// to check JWT's supplied by clients
    /// </summary>
    public static class Authorization
    {
        private static HttpClient verifyClient;
        public static async Task<string> VerifyToken(IHttpClientFactory clientFactory, string token)
        {
            using(HttpClientHandler handler = new HttpClientHandler())
            {
                // Retrieve the Environment Variable for ASP.NET Core's 
                // Environment mode
                // If it's under 'Development', allow all SSL connections to
                // avoid connections issues.
                var mode = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if(mode == "Development")
                {
                    handler.ServerCertificateCustomValidationCallback = 
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                }

                var verifyURI = Environment.GetEnvironmentVariable("JWTTOKEN_VERIFICATION_URI");
                verifyClient = new HttpClient(handler);
                verifyClient.BaseAddress = new Uri(verifyURI);

                verifyClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("bearer", token);
                    
                var response =  await verifyClient.GetAsync("");
                if(response.StatusCode == HttpStatusCode.OK)
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }

            return null;
        }
    }
}