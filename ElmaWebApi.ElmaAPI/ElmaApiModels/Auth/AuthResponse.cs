using System;

namespace ElmaWebApi.ElmaAPI.ElmaApiModels.Auth
{
    /// <summary>
    /// ELMA auth info, required tokens
    /// </summary>
    public class AuthResponse
    {
        public Guid AuthToken { get; set; }

        public string SessionToken { get; set; }
    }
}
