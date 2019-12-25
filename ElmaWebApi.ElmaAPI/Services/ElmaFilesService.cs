using System;
using System.Threading.Tasks;
using System.Web;
using ElmaWebApi.App.ExtentionAPI;
using ElmaWebApi.ElmaAPI.ElmaApiModels.Auth;
using Newtonsoft.Json;

namespace ElmaWebApi.ElmaAPI
{
    /// <summary>
    /// ELMA-files service. Used to upload/download files
    /// </summary>
    public sealed class ElmaFilesService : IService
    {
        private const string FilesRoot = "Files/";
        private readonly ElmaRestClient client;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="client">ELMA REST-client</param>
        public ElmaFilesService(ElmaRestClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="authResponse">ELMA auth info</param>
        /// <returns>Unique file identifier</returns>
        public async Task<Guid> Upload(string fileName, byte[] file, AuthResponse authResponse)
        {
            var response = await client.Post($"{FilesRoot}/Upload", authResponse, file, headers: ("FileName", HttpUtility.UrlEncode(fileName)));
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(response.StatusCode.ToString());
            }
            return JsonConvert.DeserializeObject<Guid>(await response.Content.ReadAsStringAsync());
        }
    }
}
