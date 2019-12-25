using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
using ElmaWebApi.App.ExtentionAPI;
using ElmaWebApi.ElmaAPI.ElmaApiModels.Documents;
using Newtonsoft.Json;

namespace ElmaWebApi.ElmaAPI
{
    /// <summary>
    /// ELMA-document service. Used to obtain information about documents
    /// </summary>
    public sealed class ElmaDocumentService : IService
    {
        private const string DocumentsRoot = "EleWise.ELMA.Documents/";
        private readonly ElmaRestClient client;
        private readonly ElmaAuthorizationService authorizationService;

        public ElmaDocumentService(ElmaRestClient client, ElmaAuthorizationService authorizationService)
        {
            this.client = client;
            this.authorizationService = authorizationService;
        }

        /// <summary>
        /// Get current versions of document
        /// </summary>
        public async Task<IEnumerable<DocumentVersionModel>> GetCurrentVersions(long documentId)
        {
            string contentString;
            var task = client.AutorizedGet($"{DocumentsRoot}/Document/{documentId}/CurrentVersions", ElmaAuthorizationService.AuthInfo);
            try
            {
                contentString = await task;
                if (contentString == null)
                {
                    return null;
                }
            }
            catch (AuthenticationException)
            {
                await authorizationService.CheckToken();
                contentString = await task;
            }

            return JsonConvert.DeserializeObject<IEnumerable<DocumentVersionModel>>(contentString);
        }
    }
}
