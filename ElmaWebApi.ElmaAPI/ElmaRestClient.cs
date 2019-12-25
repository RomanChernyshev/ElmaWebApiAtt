using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ElmaWebApi.App.ExtentionAPI;
using ElmaWebApi.ElmaAPI.ElmaApiModels.Auth;
using Newtonsoft.Json;

namespace ElmaWebApi.ElmaAPI
{
    /// <summary>
    /// ELMA REST-client
    /// </summary>
    public sealed class ElmaRestClient : ICustomHttpClient
    {
        private const string AuthTokenHeaderName = "AuthToken";
        private const string SessionTokenHeaderName = "HeaderName";

        public ElmaRestClient(HttpClient httpClient, string baseAddress)
        {
            httpClient.BaseAddress = new Uri(baseAddress);
            HttpClient = httpClient;
            HttpClient.DefaultRequestHeaders.Add("WebData-Version", "2.0");
        }

        public HttpClient HttpClient { get; }

        /// <summary>
        /// Create autorized get-request
        /// </summary>
        /// <param name="authResponse">ELMA auth info</param>
        /// <returns>Content text</returns>
        public Task<string> AutorizedGet(string url, AuthResponse authResponse)
        {
            return AutorizedGet(url, authResponse.AuthToken, authResponse.SessionToken);
        }

        /// <summary>
        /// Create autorized get-request
        /// </summary>
        /// <param name="authToken">ELMA auth token</param>
        /// <param name="sessionToken">ELMA session token</param>
        public async Task<string> AutorizedGet(string url, Guid authToken, string sessionToken)
        {
            if (string.IsNullOrEmpty(sessionToken))
            {
                throw new InvalidOperationException("Пустой токен сессии");
            }

            var request = new HttpRequestMessage(HttpMethod.Get, UrlValidate(url));
            request.Headers.Add(AuthTokenHeaderName, authToken.ToString());
            request.Headers.Add(SessionTokenHeaderName, sessionToken);
            var response = await HttpClient.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new AuthenticationException();
            }
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Post request. This method is available only for authorized requests
        /// </summary>
        /// <param name="authResponse">ELMA auth info</param>
        /// <param name="obj">Object for serialization</param>
        public Task<HttpResponseMessage> Post(string url, AuthResponse authResponse, object obj = null, string mimeType = null, params (string, string)[] headers)
        {
            return Post(url, obj, authResponse.AuthToken, mimeType, sessionToken: authResponse.SessionToken, headers: headers);
        }

        /// <summary>
        /// Post request. This method is available only for authorized requests
        /// </summary>
        /// <param name="obj">Object for serialization</param>
        /// <param name="authToken">ELMA auth token</param>
        /// <param name="appToken">ELMA app token</param>
        /// <param name="sessionToken">ELMA session token</param>
        public Task<HttpResponseMessage> Post(
            string url, 
            object obj = null, 
            Guid? authToken = null, 
            string mimeType = null, 
            string appToken = null, 
            string sessionToken = null,
            params (string, string)[] headers)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, UrlValidate(url));

            if (mimeType == null)
            {
                mimeType = MediaTypeNames.Application.Json;
            }

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Item1, header.Item2);
                }
            }

            if (obj != null)
            {
                if (obj is byte[] byteArr)
                {
                    request.Content = new ByteArrayContent(byteArr);
                }
                else
                {
                    request.Content = new StringContent(SerializeObject(obj, mimeType), Encoding.UTF8, mimeType);
                }
            }

            if (authToken.HasValue)
            {
                request.Headers.Add(AuthTokenHeaderName, authToken.ToString());
            }

            if (!string.IsNullOrWhiteSpace(appToken))
            {
                request.Headers.Add("ApplicationToken", appToken);
            }

            if (!string.IsNullOrWhiteSpace(sessionToken))
            {
                request.Headers.Add(SessionTokenHeaderName, sessionToken);
            }

            return HttpClient.SendAsync(request);
        }

        private Uri UrlValidate(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("Пустое значение URL-адреса");
            }
            if (!Uri.TryCreate(url, UriKind.Relative, out var absoluteUrl))
            {
                throw new FormatException($"Неверный формат URL-адреса: {url}");
            }
            return absoluteUrl;
        }

        private string SerializeObject(object obj, string mimeType)
        {
            if (mimeType == MediaTypeNames.Application.Json)
            {
                return JsonConvert.SerializeObject(obj);
            }
            else if (mimeType == MediaTypeNames.Application.Xml)
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());

                using (var stringWriter = new StringWriter())
                using (var xmlWriter = XmlWriter.Create(stringWriter))
                {
                    serializer.Serialize(xmlWriter, obj);
                    return xmlWriter.ToString();
                }
            }
            else
            {
                throw new FormatException($"Недоступное MIME-расширениe." +
                    $"Доступны {MediaTypeNames.Application.Xml} и {MediaTypeNames.Application.Json}");
            }
        }
    }
}
