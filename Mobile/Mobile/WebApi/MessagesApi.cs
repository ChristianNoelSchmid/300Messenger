﻿using Newtonsoft.Json;
using Shared.Models;
using Shared.ViewModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Mobile.WebApi
{
    public static class MessagesApi
    {
        private static readonly HttpClient _client = new HttpClient(WebApiSettings.CreateHandler());

        public static async Task<ResponseResult<MessageSession>> CreateMessageSession(string jwt, string title, string description, List<string> emails)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "https://10.0.2.2:5003/MessageSession/CreateSession"))
            {
                var viewModel = new MessageSessionCreateViewModel
                {
                    FromJwt = jwt,
                    Title = title,
                    Description = description,
                    Emails = emails
                };

                request.Content = new StringContent(JsonConvert.SerializeObject(viewModel), Encoding.UTF8, "application/json");
                var response = await _client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return new ResponseResult<MessageSession>(
                        true,
                        JsonConvert.DeserializeObject<MessageSession>(await response.Content.ReadAsStringAsync())
                    );
                }

                return new ResponseResult<MessageSession>(false, null);
            }
        }

        public static async Task<ResponseResult<MessageSession[]>> GetMessageSessionsForUser(string jwt)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, "https://10.0.2.2:5003/MessageSession/GetSessions"))
            {
                var viewModel = new AuthorizedJwtViewModel { JwtFrom = jwt };
                request.Content = new StringContent(JsonConvert.SerializeObject(viewModel), Encoding.UTF8, "application/json");

                var response = await _client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return new ResponseResult<MessageSession[]>(
                        true,
                        JsonConvert.DeserializeObject<MessageSession[]>(await response.Content.ReadAsStringAsync())
                    );
                }
            }

            return new ResponseResult<MessageSession[]>(
                false,
                null
            );
        }
    
        public static async Task<ResponseResult<Message[]>> GetMessages(string jwt, int sessionId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, "https://10.0.2.2:5003/MessageSession/GetMessages"))
            {
                var viewModel = new AuthorizedIntViewModel { JwtFrom = jwt, Value = sessionId };
                request.Content = new StringContent(JsonConvert.SerializeObject(viewModel), Encoding.UTF8, "application/json");

                var response = await _client.SendAsync(request);
                if(response.IsSuccessStatusCode)
                {
                    return new ResponseResult<Message[]>(
                        true,
                        JsonConvert.DeserializeObject<Message[]>(await response.Content.ReadAsStringAsync())
                    );
                }
                return new ResponseResult<Message[]>(false, null);
            }
        }

        public static async Task<ResponseResult> AddMessageToSession(string jwt, int sessionId, string content)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "https://10.0.2.2:5003/MessageSession/AddMessage"))
            {
                var viewModel = new AuthorizedMessageViewModel
                {
                    JwtFrom = jwt,
                    Content = content,
                    SessionId = sessionId,
                    Type = MessageType.Text
                };

                request.Content = new StringContent(JsonConvert.SerializeObject(viewModel), Encoding.UTF8, "application/json");

                var response = await _client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                    return new ResponseResult(true);

                return new ResponseResult(false);
            }
        }

        public static async Task<ResponseResult> DeleteSession(string jwt, int sessionId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "https://10.0.2.2:5003/MessageSession/DeleteSession"))
            {
                var viewModel = new AuthorizedIntViewModel
                {
                    JwtFrom = jwt,
                    Value = sessionId
                };

                request.Content = new StringContent(JsonConvert.SerializeObject(viewModel), Encoding.UTF8, "application/json");

                var response = await _client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                    return new ResponseResult(true);

                return new ResponseResult(false);
            }
        }

    }
}