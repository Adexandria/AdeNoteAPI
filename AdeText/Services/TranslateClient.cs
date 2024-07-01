using AdeText.Models;
using AdeText.Utilities;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;


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
            _jsonExtractorService = new JsonExtractorService();
        }
        public async Task<IDetectLanguage> DetectLanguage(string text, CancellationToken cancellationToken)
        {
            object[] body = new object[] { new { Text = text } };

            var requestBody = JsonSerializer.Serialize(body);
            return await SendRequest<DetectLanguage>(requestBody, "detect?api-version=3.0", cancellationToken);
        }

        public async Task<ITranslateLanguage> TranslateLanguage(string text, string to, string from = null, CancellationToken cancellationToken = default)
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

            return await SendRequest<TranslateLanguage>(requestBody, endpoint, cancellationToken);
        }

        public async Task<ITranslateLanguage> TranslateLanguage(string text, string[] tos, string from = null, CancellationToken cancellationToken = default)
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

            return await SendRequest<TranslateLanguage>(requestBody, endpoint, cancellationToken);
        }


        public async Task<Translation> TransliterateLanguage(string text, string toLanguage, string fromScript, CancellationToken cancellationToken = default )
        {
            object[] body = new object[] { new { Text = text } };

            var requestBody = JsonSerializer.Serialize(body);

            string endpoint = $"transliterate?api-version=3.0" +
                $"&language={toLanguage}&fromScript={fromScript}&toScript=Latn";

            return await SendRequest<Translation>(requestBody, endpoint, cancellationToken);
        }


        public ILanguage GetSupportedLanguages(string[] scopes, string _etag = null, CancellationToken cancellationToken = default)
        {
            try
            {
                for(int i = 0; i < scopes.Length; i++)
                {
                    if(i == 2)
                    {
                        break;
                    }
                    var isScope = Enum.TryParse(scopes[i], out Scope result);
                    if (!isScope)
                    {
                        throw new Exception("Invalid scope");
                    }
                }
                using var client = new HttpClient();
                using var request = new HttpRequestMessage();
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(_endpoint + $"languages?api-version=3.0&scope={scopes[0]},{scopes[1]}");

                if(!string.IsNullOrEmpty(_etag))
                {
                    request.Headers.IfNoneMatch
                        .Add(new System.Net.Http.Headers.EntityTagHeaderValue(_etag));
                }

                HttpResponseMessage response = client.Send(request,cancellationToken);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new TranslationException(response.StatusCode);
                }

                if(response.StatusCode == HttpStatusCode.NotModified)
                {
                    return new TextLanguage(_etag);
                }
                var content = response.Content.ReadAsStreamAsync().Result;

                var deserialiseContent = _jsonExtractorService.ExtractJson<Root>(content);

                var etag = response.Headers.FirstOrDefault(s => s.Key == "ETag").Value.FirstOrDefault();

                return new TextLanguage(deserialiseContent.TranslationLanguage.Languages, 
                    deserialiseContent.TransliterationLanguage.Languages, etag);
            }
            catch (TranslationException ex)
            {
                if (ex.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return null;
                }
                Task.Delay(TimeSpan.FromSeconds(Math.Pow(3, requestSent))).Wait();
                requestSent++;
                if (requestSent < retryConfiguration && ex.StatusCode == HttpStatusCode.TooManyRequests 
                    || ex.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    return GetSupportedLanguages(scopes,_etag);
                }
                else
                {
                    return default;
                }

            }
            finally
            {
                requestSent = 0;
            }

        }



        private async Task<TConcrete> SendRequest<TConcrete>(string requestBody, string endpoint, CancellationToken cancellationToken) 
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

                HttpResponseMessage response = await client.SendAsync(request, cancellationToken);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new TranslationException(response.StatusCode);
                }

                var content = await response.Content.ReadAsStringAsync();

                var jsonObject = JsonNode.Parse(content);

                TConcrete deserialiseContent;

                if (jsonObject is JsonArray)
                {
                    var jsonArray = jsonObject.AsArray();

                    deserialiseContent = JsonSerializer.Deserialize<TConcrete>(jsonArray[0]);

                }
                else
                {
                    deserialiseContent = JsonSerializer.Deserialize<TConcrete>(content);
                }

                return deserialiseContent;
            }
            catch (TranslationException ex)
            {
                if(ex.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return null;
                }
                Task.Delay(TimeSpan.FromSeconds(Math.Pow(3, requestSent)), cancellationToken).Wait();
                requestSent++;
                if (requestSent < retryConfiguration && ex.StatusCode == HttpStatusCode.TooManyRequests
                    || ex.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    return await SendRequest<TConcrete>(requestBody, endpoint,cancellationToken);
                }
                else
                {
                    return default;
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
        private readonly IJsonExtractorService _jsonExtractorService;
       
    }
}
