using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Olav.Sanity.Client.Mutators;

namespace Olav.Sanity.Client
{
    public class SanityClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _projectId;
        private readonly string _dataset;
        private readonly string _token;
        private readonly bool _useCdn;

        public enum Visibility { Sync, Async, Deferred }

        private bool _disposed;

        /// <summary>
        /// </summary>
        /// <param name="projectId">The sanity project id</param>
        /// <param name="dataset">The dataset name you want to query/mutate. Defined in your sanity project</param>
        /// <param name="token">Auth token, get this from the sanity project</param>
        /// <param name="useCdn">The sanity project id</param>
        public SanityClient(string projectId,
                            string dataset,
                            string token,
                            bool useCdn)
            : this(projectId, dataset, token, useCdn, new HttpClientHandler())
        {            
        }

        public SanityClient(string projectId,
                            string dataset,
                            string token,
                            bool useCdn,
                            HttpMessageHandler innerHttpMessageHandler)
        {
            if (string.IsNullOrEmpty(projectId)) throw new ArgumentNullException(nameof(projectId));
            if (string.IsNullOrEmpty(dataset)) throw new ArgumentNullException(nameof(dataset));
            if (innerHttpMessageHandler == null) throw new ArgumentNullException(nameof(innerHttpMessageHandler));

            _projectId = projectId; 
            _dataset = dataset;
            _token = token;
            _useCdn = useCdn;


            _httpClient = new HttpClient(innerHttpMessageHandler);

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            _httpClient.BaseAddress = new Uri($"https://{projectId}.api.sanity.io/v1/data/");
        }

        /// <summary>
        /// Get a single document by id
        /// </summary>
        /// <param name="id">Document id</param>
        public virtual async Task<(HttpStatusCode, T)> GetDocument<T>(string id) where T : class
        {
            var message = await _httpClient.GetAsync($"doc/{_dataset}/{id}");
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

        /// <summary>
        /// Fetch documents using a GROQ query
        /// </summary>
        /// <param name="query">GROQ query</param>
        public virtual async Task<(HttpStatusCode, T)> Fetch<T>(string query) where T : class
        {
            var encodedQ = System.Net.WebUtility.UrlEncode(query);
            var message = await _httpClient.GetAsync($"query/{_dataset}?query={encodedQ}");
            return await ResponseToResult<T>(message);
        }

        /// <summary>
        /// Change one or more document using the given Mutations
        /// </summary>
        /// <param name="mutations">Mutations object containing mutations</param>
        /// <param name="returnIds">If true, the id's of modified documents are returned</param>
        /// <param name="returnDocuments">If true, the entire content of changed documents is returned</param>
        /// <param name="visibility">If "sync" the request will not return until the requested changes are visible to subsequent queries, if "async" the request will return immediately when the changes have been committed. For maximum performance, use "async" always, except when you need your next query to see the changes you made. "deferred" is used in cases where you are adding or mutating a large number of documents and don't need them to be immediately available.</param>
        public virtual async Task<(HttpStatusCode, string)> Mutate(
            Mutations mutations, bool returnIds = false, bool returnDocuments = false,
            Visibility visibility = Visibility.Sync)
        {
            var content = new StringContent(mutations.Serialize());
            var message = await _httpClient.PostAsync($"mutate/{_dataset}?returnIds={returnIds}&returnDocuments={returnDocuments}&visibiliy={visibility.ToString().ToLower()}", content);
            return await ResponseToResult<string>(message);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                _httpClient.Dispose();
            }
        }
    }
}