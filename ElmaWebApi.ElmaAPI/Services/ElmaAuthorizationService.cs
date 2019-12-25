using System;
using System.Security.Authentication;
using System.Threading.Tasks;
using ElmaWebApi.App.ExtentionAPI;
using ElmaWebApi.ElmaAPI.ElmaApiModels.Auth;
using Newtonsoft.Json;

namespace ElmaWebApi.ElmaAPI
{
    /// <summary>
    /// ELMA-authorization service. Serves for authorization and updating of tokens
    /// </summary>
    public sealed class ElmaAuthorizationService : IService
    {
        private const string AuthorizationRoot = "Authorization/";
        private readonly ElmaRestClient client;

        public static AuthResponse AuthInfo { get; private set; }

        public ElmaAuthorizationService(ElmaRestClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Login in ELMA
        /// </summary>
        /// <returns>ELMA auth info</returns>
        public async Task<AuthResponse> LoginWithUserName(string username, string password, string appToken)
        {
            if (string.IsNullOrWhiteSpace(appToken))
            {
                throw new InvalidOperationException("Пустое значение токена приложения");
            }

            var response = await client.Post($"{AuthorizationRoot}LoginWith?username={username}",obj:string.Empty, appToken: appToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new AuthenticationException("Ошибка авторизации в ELMA");
            }

            var contentString = await response.Content.ReadAsStringAsync();
            var authResponse = JsonConvert.DeserializeObject<AuthResponse>(contentString);
            AuthInfo = authResponse;
            return authResponse;
        }

        /// <summary>
        /// Update ELMA-tokens
        /// </summary>
        /// <returns>ELMA auth info</returns>
        public async Task<AuthResponse> CheckToken()
        {
            var contentString = await client.AutorizedGet($"{AuthorizationRoot}/CheckToken?token={AuthInfo.AuthToken}", AuthInfo);
            if (contentString == null)
            {
                return null;
            }

            var authResponse = JsonConvert.DeserializeObject<AuthResponse>(contentString);
            AuthInfo = authResponse;
            return authResponse;
        }
    }
}
