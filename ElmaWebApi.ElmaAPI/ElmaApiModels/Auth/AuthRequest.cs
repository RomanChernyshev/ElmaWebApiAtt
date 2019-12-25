namespace ElmaWebApi.ElmaAPI.ElmaApiModels.Auth
{
    /// <summary>
    /// Authorization request
    /// </summary>
    public class AuthRequest
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
}
