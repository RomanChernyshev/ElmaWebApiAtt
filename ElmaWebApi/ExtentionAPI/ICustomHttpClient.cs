using System.Net.Http;

namespace ElmaWebApi.App.ExtentionAPI
{
    /// <summary>
    /// Marker-interface for register in app IoC-container custom Http-client
    /// </summary>
    public interface ICustomHttpClient
    {
        HttpClient HttpClient { get; }
    }
}
