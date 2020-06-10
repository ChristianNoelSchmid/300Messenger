using _300Messenger.Shared.ViewModels;
using Newtonsoft.Json;
using Shared.Models;
using Shared.ViewModels;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Mobile.WebApi
{   
    public static class AccountsApi
    {
        private static readonly HttpClient _client = new HttpClient(WebApiSettings.CreateHandler());

        public static async Task<ResponseResult<string>> Register(RegisterViewModel viewModel)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://52.12.195.150:5005/Account/Register");

            request.Content = new StringContent(JsonConvert.SerializeObject(viewModel), Encoding.UTF8, "application/json");
            var response = await _client.SendAsync(request);

            if(response.StatusCode == HttpStatusCode.OK)
                return new ResponseResult<string>(true, null);
            else
                return new ResponseResult<string>(false, await response.Content.ReadAsStringAsync());
        }

        public static async Task<ResponseResult<string>> GetJwt(string email, string password)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://52.12.195.150:5005/Account/Login");
            var viewModel = new LoginViewModel()
            {
                Email = email,
                Password = password
            };
            request.Content = new StringContent(JsonConvert.SerializeObject(viewModel), Encoding.UTF8, "application/json");
            var response = await _client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return new ResponseResult<string>(true, content);
            }
            return new ResponseResult<string>(false, content);
        }

        public static async Task<ResponseResult<User>> GetUserByJwt(string jwt)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, "https://52.12.195.150:5005/Account/GetUserByJwt"))
            {
                var viewModel = new AuthorizedJwtViewModel { JwtFrom = jwt };
                request.Content = new StringContent(JsonConvert.SerializeObject(viewModel), Encoding.UTF8, "application/json");

                var response = await _client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return new ResponseResult<User>(
                        true,
                        JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync())
                    );
                }
            }

            return new ResponseResult<User>(false, null);
        }

        public static async Task<ResponseResult<User>> GetUserByEmail(string jwt, string email)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, "https://52.12.195.150:5005/Account/GetUserByEmail"))
            {
                var viewModel = new AuthorizedEmailViewModel
                {
                    JwtFrom = jwt,
                    Email = email
                };

                request.Content = new StringContent(JsonConvert.SerializeObject(viewModel), Encoding.UTF8, "application/json");

                var response = await _client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return new ResponseResult<User>(
                        true,
                        JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync())
                    );
                }
            }

            return new ResponseResult<User>(false, null);
        }        

        public static async Task<ResponseResult<User[]>> GetUsersByQuery(string jwt, string query)
        {
            using(var request = new HttpRequestMessage(HttpMethod.Get, "https://52.12.195.150:5005/Account/GetUsers"))
            {
                var viewModel = new AuthorizedQueryViewModel { JwtFrom = jwt, Value = query };
                request.Content = new StringContent(JsonConvert.SerializeObject(viewModel), Encoding.UTF8, "application/json");

                var response = await _client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return new ResponseResult<User[]>(
                        true,
                        JsonConvert.DeserializeObject<User[]>(await response.Content.ReadAsStringAsync())
                    );
                }
            }

            return new ResponseResult<User[]>(false, null);
        }
    }
}
