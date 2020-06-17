using Newtonsoft.Json;
using Plugin.Media.Abstractions;
using Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Mobile.WebApi
{
    public static class ImagesApi
    {
        private static readonly HttpClient _client = new HttpClient(WebApiSettings.CreateHandler());
        private static readonly string URI = WebApiSettings.DevUriRoutes["Images"];
        public static async Task<ResponseResult<ImageSource>> GetProfileImage(string email, bool isThumb)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"{URI}/GetProfileImage"))
            {

                var viewModel = new AuthorizedProfileImageViewModel
                {
                    Email = email,
                    IsThumb = isThumb
                };
                request.Content = new StringContent(JsonConvert.SerializeObject(viewModel), Encoding.UTF8, "application/json");

                var response = await _client.SendAsync(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return new ResponseResult<ImageSource>(
                        true,
                        ImageSource.FromStream(() => response.Content.ReadAsStreamAsync().Result)
                    );
                }

                return new ResponseResult<ImageSource>(false, ImageSource.FromFile("default_profile.png"));
            }
        }

        public static async Task<ResponseResult<byte[]>> GetProfileImagePersistent(string email, bool isThumb)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"{URI}/GetProfileImage"))
            {
                var viewModel = new AuthorizedProfileImageViewModel
                {
                    Email = email,
                    IsThumb = isThumb
                };
                request.Content = new StringContent(JsonConvert.SerializeObject(viewModel), Encoding.UTF8, "application/json");

                var response = await _client.SendAsync(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return new ResponseResult<byte[]>(
                        true,
                        await response.Content.ReadAsByteArrayAsync()
                    );
                }
                else return new ResponseResult<byte[]>(
                    false,
                    null
                );
            }
        }

        public static async Task<ResponseResult> PostProfileImage(string jwt, MediaFile image)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"{URI}/PostProfileImage"))
            {
                var viewModel = new AuthorizedJwtViewModel { JwtFrom = jwt };

                var stream = image.GetStream();
                byte[] imageBytes = new byte[stream.Length];
                await stream.ReadAsync(imageBytes, 0, imageBytes.Length);

                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(JsonConvert.SerializeObject(viewModel), Encoding.UTF8, "application/json"), "Auth");
                    content.Add(new ByteArrayContent(imageBytes), "files", Path.GetFileName(image.Path));
                    request.Content = content;

                    var response = await _client.SendAsync(request);

                    return new ResponseResult(response.StatusCode == HttpStatusCode.OK);
                }
            }
        }
    }
}
