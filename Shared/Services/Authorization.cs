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
        public static HttpClient CreateVerificationClient(IHttpClientFactory clientFactory)
        {
            var client = clientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:5005/Account/Verify");
            return client;
        }
        public static async Task<string> VerifyToken(HttpClient verifyClient, string token)
        {
            verifyClient.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

            var response =  await verifyClient.GetAsync("");
            if(response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return null;
        }
    }
}