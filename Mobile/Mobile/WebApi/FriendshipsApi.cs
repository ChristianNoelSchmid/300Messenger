using Newtonsoft.Json;
using Shared.Models;
using Shared.ViewModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Mobile.WebApi
{
    public enum FriendStatus { IsNotConfirmed, IsConfirmed, NotFriend }
    public static class FriendshipsApi
    {
        private static readonly HttpClient _client = new HttpClient(WebApiSettings.CreateHandler());
        private static readonly string URI = WebApiSettings.ServerUriRoutes["Friendships"];

        public static async Task<ResponseResult<Friendship>> GetFriendship(string jwt, string email)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, "https://10.0.2.2:5001/Friendship/GetFriendship"))
            {
                var viewModel = new AuthorizedEmailViewModel { Email = email, JwtFrom = jwt };
                request.Content = new StringContent(JsonConvert.SerializeObject(viewModel), Encoding.UTF8, "application/json");

                var response = await _client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return new ResponseResult<Friendship>(true, JsonConvert.DeserializeObject<Friendship>(await response.Content.ReadAsStringAsync()));
                }
                else if (await response.Content.ReadAsStringAsync() == "Does Not Exist")
                {
                    return new ResponseResult<Friendship>(true, null);
                }
                else
                {
                    return new ResponseResult<Friendship>(false, null);
                }
            }
        }

        public static async Task<ResponseResult<Friendship[]>> GetUserFriends(string jwt)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{URI}/GetFriendships");
            var viewModel = new AuthorizedJwtViewModel { JwtFrom = jwt };

            request.Content = new StringContent(JsonConvert.SerializeObject(viewModel), Encoding.UTF8, "application/json");
            var response = await _client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                if (response.Content != null)
                    return new ResponseResult<Friendship[]>(
                        true,
                        JsonConvert.DeserializeObject<Friendship[]>(await response.Content.ReadAsStringAsync())
                    );
                else return new ResponseResult<Friendship[]>(true, new Friendship[0]);
            }

            return new ResponseResult<Friendship[]>(false, new Friendship[0]);
        }
    
        public static async Task<ResponseResult> CreateFriendship(string jwt, string email)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "{URI}/Create");
            var viewModel = new AuthorizedEmailViewModel { JwtFrom = jwt, Email = email };

            request.Content = new StringContent(JsonConvert.SerializeObject(viewModel), Encoding.UTF8, "application/json");
            var response = await _client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return new ResponseResult(true);
            }

            return new ResponseResult(false);
        }

        public static async Task<ResponseResult> RemoveFriendship(string jwt, int id)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "{URI}/Remove");
            var viewModel = new AuthorizedIntViewModel { JwtFrom = jwt, Value = id };

            request.Content = new StringContent(JsonConvert.SerializeObject(viewModel), Encoding.UTF8, "application/json");
            var response = await _client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return new ResponseResult(true);
            }

            return new ResponseResult(false);
        }

        public static async Task<ResponseResult> ConfirmFriendship(string jwt, int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, "{URI}/Confirm");
            var viewModel = new AuthorizedIntViewModel { JwtFrom = jwt, Value = id };

            request.Content = new StringContent(JsonConvert.SerializeObject(viewModel), Encoding.UTF8, "application/json");
            var response = await _client.SendAsync(request);

            if(response.IsSuccessStatusCode)
            {
                return new ResponseResult(true);
            }

            return new ResponseResult(false);
        }
    }
}
