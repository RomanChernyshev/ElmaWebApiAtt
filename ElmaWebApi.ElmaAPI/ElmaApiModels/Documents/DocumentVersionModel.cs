using System;
using ElmaWebApi.ElmaAPI.ElmaApiModels.Files;

namespace ElmaWebApi.ElmaAPI.ElmaApiModels.Documents
{
    /// <summary>
    /// Document version for file
    /// </summary>
    public class DocumentVersionModel : ElmaEntity
    {
        public DocumentVersionModel(ElmaFile file, long version)
        {
            this.File = file;
            this.Version = version;
        }

        public override Guid TypeUid { get => new Guid("ec6a6443-38ce-4a90-8d2d-55b7e564cd31"); }

        public long Version { get; }

        public ElmaFile File { get; }
    }
}
