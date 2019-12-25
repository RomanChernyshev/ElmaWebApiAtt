using ElmaWebApi.App.ExtentionAPI;

namespace ElmaWebApi.App.Core.Configurations
{
    /// <summary>
    /// Main external api settings
    /// </summary>
    public sealed class ExternalApi : IExternalApiDescriptor
    {
        public string Url { get; set; }

        public string ClientIdentifier { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string ApplicationToken { get; set; }
    }
}
