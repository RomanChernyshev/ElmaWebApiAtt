using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using ElmaWebApi.App.ExtentionAPI;
using ElmaWebApi.App.Models;
using ElmaWebApi.ElmaAPI;
using ElmaWebApi.ElmaAPI.ElmaApiModels.Documents;
using ElmaWebApi.ElmaAPI.Managers;

namespace ElmaWebApi.ElmaFileWatcher
{
    /*
     * When changing, it checks for the presence of the same file in ELMA.
     * If the file is not found, a new document entry with this file will be created in the ELMA database.
     * Else a new version of the document with this file will be created.
     */
    
    /// <summary>
    /// The implementation of the directory handler for ELMA WEB-API. 
    /// </summary>
    public sealed class ElmaFileCreateWatcher : IFileWatchHandler
    {
        private readonly IEnumerable<IExternalApiDescriptor> apiDescriptors;
        private readonly ElmaAuthorizationService elmaAuthorizationService;
        private readonly ElmaEntityManager elmaEntityManager;
        private readonly ElmaFilesService elmaFilesService;

        public ElmaFileCreateWatcher(
            IEnumerable<IExternalApiDescriptor> apiDescriptors,
            ElmaAuthorizationService elmaAuthorizationService,
            ElmaEntityManager elmaEntityManager,
            ElmaFilesService elmaFilesService)
        {
            this.apiDescriptors = apiDescriptors;
            this.elmaAuthorizationService = elmaAuthorizationService;
            this.elmaEntityManager = elmaEntityManager;
            this.elmaFilesService = elmaFilesService;
        }

        public WatcherChangeTypes[] ChangeTypes => new[] { WatcherChangeTypes.Created };

        public async Task Execute(FileDescription fileDescription)
        {
            var fileName = Path.GetFileName(fileDescription.FullPath);
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            var apiExt = apiDescriptors.FirstOrDefault(a => a.ClientIdentifier == "ElmaRestClient");

            if (ElmaAuthorizationService.AuthInfo == null 
                || ElmaAuthorizationService.AuthInfo.AuthToken == Guid.Empty 
                || string.IsNullOrEmpty(ElmaAuthorizationService.AuthInfo.SessionToken))
            {
                await elmaAuthorizationService.LoginWithUserName(apiExt.Login, apiExt.Password, apiExt.ApplicationToken);
            }

            var result = await elmaEntityManager.QueryByMatch(new ElmaDocument(fileName), ElmaAuthorizationService.AuthInfo);

            var file = await ReadFile(fileDescription.FullPath);
            var fileUid = await elmaFilesService.Upload(fileName, file, ElmaAuthorizationService.AuthInfo);

            if (result == null || !result.Any())
            {
                await elmaEntityManager.Insert(new ElmaDocument(fileName)
                {
                    Versions = new List<DocumentVersionModel>
                    {
                        new DocumentVersionModel(new ElmaAPI.ElmaApiModels.Files.ElmaFile
                            {
                                Name = fileName,
                                Uid = fileUid
                            }, default)
                    }
                });
            }
            else
            {
                var existingDocument = result.First();

                existingDocument.Versions = existingDocument.Versions 
                    ?? new List<DocumentVersionModel>();

                var versionNumber = existingDocument.Versions.Any() 
                    ? existingDocument.Versions.Max(v => v.Version) + 1 
                    : default;

                var version = new DocumentVersionModel(new ElmaAPI.ElmaApiModels.Files.ElmaFile
                {
                    Name = fileName,
                    Uid = fileUid
                }, versionNumber);

                version.Id = await elmaEntityManager.Insert(version);

                existingDocument.Versions.Add(version);

                await elmaEntityManager.Update(existingDocument);
            }

            Console.WriteLine(fileDescription.FullPath);
        }


        bool authorizationFailed = false;
        public async Task CatchException(Exception exception, FileDescription fileDescription)
        {
            if (authorizationFailed)
            {
                throw new InvalidDataException("Неверные учетные данные");
            }
            if (exception is AuthenticationException)
            {
                await elmaAuthorizationService.CheckToken();
            }
            authorizationFailed = true;
            await Execute(fileDescription);
        }

        private async Task<byte[]> ReadFile(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            int length = Convert.ToInt32(fs.Length);
            byte[] data = new byte[length];
            await fs.ReadAsync(data, 0, length);
            fs.Close();
            return data;
        }
    }
}
