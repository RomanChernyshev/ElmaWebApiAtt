using System;
using System.Collections.Generic;

namespace ElmaWebApi.ElmaAPI.ElmaApiModels.Documents
{
    public class ElmaDocument : ElmaEntity
    {
        public ElmaDocument(string name)
        {
            this.Name = name;
        }

        public override Guid TypeUid { get => new Guid("ff9b2cc5-861b-4991-abaa-2aa1141a7566"); }

        public string Name { get; }

        public List<DocumentVersionModel> Versions { get; set; }
    }
}
