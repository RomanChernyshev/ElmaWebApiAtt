namespace ElmaWebApi.App.ExtentionAPI
{
    /// <summary>
    /// Marker-interface for expand external api settings
    /// </summary>
    public interface IExternalApiDescriptor
    {
        string Url { get; }

        string ClientIdentifier { get; }

        string Login { get; }

        string Password { get; }

        string ApplicationToken { get; set; }
    }
}
