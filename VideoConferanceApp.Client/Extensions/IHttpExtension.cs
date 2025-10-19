using NetcodeHub.Packages.Extensions.LocalStorage;
using System.Net.Http.Headers;

namespace VideoConferanceApp.Client.Extensions
{
    public interface IHttpExtension
    {
        HttpClient GetPublicClient();
        Task<HttpClient> GetPrivateClient();
    }

    public class HttpExtension(IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService, IConfiguration config) : IHttpExtension
    {
        HttpClient CreateClient() => httpClientFactory.CreateClient(config["HttpClient:Name"]!);
        public async Task<HttpClient> GetPrivateClient()
        {
            var client = CreateClient();
            string key = config["Token:Key"]!;
            if (key == null) return client;

            string token = await localStorageService.GetItemAsStringAsync(key);
            if (key != null)
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return client;
        }

        public HttpClient GetPublicClient() => CreateClient();
    }
}
