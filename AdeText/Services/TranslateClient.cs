﻿using AdeText.Models;
using System.Net;
using System.Text;
using System.Text.Json;


namespace AdeText.Services
{
    internal class TranslateClient : ITranslateClient
    {
        public TranslateClient(ITranslateConfiguration _configuration)
        {
            _key = _configuration.Key ?? throw new NullReferenceException(nameof(_configuration.Key));
            _endpoint = _configuration.Endpoint ?? throw new NullReferenceException(nameof(_configuration.Endpoint));
            _location = _configuration.Location ?? throw new NullReferenceException(nameof(_configuration.Location));
            retryConfiguration = _configuration.RetryConfiguration  == 0 ? 3 : _configuration.RetryConfiguration;
            requestSent = 0;
        }
        public async Task<IDetectLanguage> DetectLanguage(string text)
        {
            object[] body = new object[] { new { Text = text } };

            var requestBody = JsonSerializer.Serialize(body);
            return await SendRequest<DetectLanguage>(requestBody, "detect?api-version=3.0");
        }

        public async Task<ITranslateLanguage> TranslateLanguage(string text, string to, string from = null)
        {
            object[] body = new object[] { new { Text = text } };

            var requestBody = JsonSerializer.Serialize(body);

            string endpoint = "translate?api-version=3.0&";

            if (from != null)
            {
                endpoint += $"from={from}&to={to}";
            }
            else
            {
                endpoint += $"to={to}";
            }

            return await SendRequest<TranslateLanguage>(requestBody, endpoint);
        }

        public async Task<ITranslateLanguage> TranslateLanguage(string text, string[] tos, string from = null)
        {
            object[] body = new object[] { new { Text = text } };

            var requestBody = JsonSerializer.Serialize(body);

            string endpoint = "translate?api-version=3.0&";

            if (from != null)
            {
                endpoint += $"from={from}";
            }

            foreach (var to in tos)
            {
                endpoint += $"&to={to}";
            }

            return await SendRequest<TranslateLanguage>(requestBody, endpoint);
        }



        public ILanguage GetSupportedTranslationLanguages
        {
            get
            {
                try
                {
                    using var client = new HttpClient();
                    using var request = new HttpRequestMessage();
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(_endpoint + "languages?api-version=3.0&scope=translation");

                    HttpResponseMessage response = client.Send(request);

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception();
                    }
                    var content = response.Content.ReadAsStringAsync().Result;

                    var deserialiseContent = JsonSerializer.Deserialize<Dictionary<string, SupportedLanguage>>(content);

                    var etag = response.Headers.FirstOrDefault(s => s.Key == "ETag").Value.FirstOrDefault();

                    return new Language(deserialiseContent,etag);
                }
                catch (Exception ex)
                {
                    Task.Delay(TimeSpan.FromSeconds(Math.Pow(3, requestSent))).Wait();
                    requestSent++;
                    if(requestSent < retryConfiguration)
                    {
                        return GetSupportedTranslationLanguages;
                    }
                }
                finally
                {
                    requestSent = 0;
                }
            }
        }



        private async Task<TConcrete> SendRequest<TConcrete>(string requestBody, string endpoint) 
            where TConcrete : class
        {
            try
            {
                using var client = new HttpClient();
                using var request = new HttpRequestMessage();
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(_endpoint + endpoint);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", _key);
                request.Headers.Add("Ocp-Apim-Subscription-Region", _location);

                HttpResponseMessage response = await client.SendAsync(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception();
                }

                var content = await response.Content.ReadAsStringAsync();

                var deserialiseContent = JsonSerializer.Deserialize<TConcrete>(content);

                return deserialiseContent;
            }
            catch (Exception ex)
            {
                Task.Delay(TimeSpan.FromSeconds(Math.Pow(3, requestSent))).Wait();
                requestSent++;
                if (requestSent < retryConfiguration)
                {
                    return await SendRequest<TConcrete>(requestBody, endpoint);
                }
            }
            finally
            {
                requestSent = 0;
            }
        }

     

        private int requestSent;
        private int retryConfiguration;
        private string _key;
        private string _endpoint;
        private string _location;

       
    }
}
