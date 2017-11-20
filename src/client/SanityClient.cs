using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Olav.Sanity.Client.Mutators;

namespace Olav.Sanity.Client
{
    public class SanityClient
    {
        private static readonly HttpClient HttpClient;
        private readonly string _projectId;
        private readonly string _dataset;
        private readonly string _token;
        private readonly bool _useCdn;

        public enum Visibility { Sync, Async, Deferred }

        static SanityClient()
        {
            var handler = new HttpClientHandler();

            HttpClient = new HttpClient(handler);
            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public SanityClient(string projectId,
                            string dataset,
                            string token,
                            bool useCdn)
        {
            _projectId = projectId;
            _dataset = dataset;
            _token = token;
            _useCdn = useCdn;
            HttpClient.BaseAddress = new Uri($"https://{projectId}.api.sanity.io/v1/data/");
        }

        public async Task<(HttpStatusCode, T)> GetDocument<T>(string id) where T : class
        {
            var message = await HttpClient.GetAsync($"doc/{_dataset}/{id}");
            return await ResponseToResult<T>(message);
        }

        private async Task<(HttpStatusCode, T)> ResponseToResult<T>(HttpResponseMessage message) where T : class
        {
            if (!message.IsSuccessStatusCode)
            {
                return (message.StatusCode, null);
            }
            var content = await message.Content.ReadAsStringAsync();
            return (message.StatusCode, JsonConvert.DeserializeObject<T>(content));
        }

        public async Task<(HttpStatusCode, T)> Fetch<T>(string query) where T : class
        {
            var message = await HttpClient.GetAsync($"query/{_dataset}?query={query}");
            return await ResponseToResult<T>(message);
        }

        public async Task<(HttpStatusCode, string)> Mutate(
            Mutations mutations, bool returnIds = false, bool returnDocuments = false,
            Visibility visibility = Visibility.Sync)
        {
            var content = new StringContent(mutations.Serialize());
            var message = await HttpClient.PostAsync($"mutate/{_dataset}?returnIds={returnIds}&returnDocuments={returnDocuments}&visibiliy={visibility.ToString().ToLower()}", content);
            return await ResponseToResult<string>(message);
        }
    }
}
