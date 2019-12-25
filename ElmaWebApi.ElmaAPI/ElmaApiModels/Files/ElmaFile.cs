using System;
using ElmaWebApi.ElmaAPI.ElmaApiModels.Documents;

namespace ElmaWebApi.ElmaAPI.ElmaApiModels.Files
{
    public class ElmaFile
    {
        public Guid Uid { get; set; }

        public string Name { get; set; }

        public DocumentVersionModel Versions { get; set; }
    }
}
