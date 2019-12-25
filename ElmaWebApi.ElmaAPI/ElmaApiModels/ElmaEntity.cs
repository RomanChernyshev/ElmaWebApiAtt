using System;

namespace ElmaWebApi.ElmaAPI.ElmaApiModels
{
    public class ElmaEntity
    {
        public virtual Guid TypeUid { get; }

        public long Id { get; set; }
    }
}
