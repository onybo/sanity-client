using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Olav.Sanity.Client.Extensions;
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
            if (!string.IsNullOrEmpty(_token))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }

        /// <summary>
        /// Get a single document by id
        /// </summary>
        /// <param name="id">Document id</param>
        /// <returns>Tuple of HttpStatusCode and a T wrapped in a DocumentResult</returns>
        public virtual async Task<(HttpStatusCode, DocumentResult<T>)> GetDocument<T>(string id) where T : class
        {
            var message = await _httpClient.GetAsync($"doc/{_dataset}/{id}").ConfigureAwait(false);
            return await ResponseToResult<DocumentResult<T>>(message).ConfigureAwait(false);
        }

        private async Task<(HttpStatusCode, T)> ResponseToResult<T>(HttpResponseMessage message) where T : class
        {
            if (!message.IsSuccessStatusCode)
            {
                return (message.StatusCode, null);
            }
            var content = await message.Content.ReadAsStringAsync().ConfigureAwait(false);

            return (message.StatusCode, JsonConvert.DeserializeObject<T>(content));
        }

        [Obsolete("Use GetDocuments method instead.")]
        public virtual Task<(HttpStatusCode, FetchResult<T>)> Fetch<T>(string query, bool excludeDrafts = true)
        {
            return GetDocuments<T>(query, excludeDrafts);
        }

        /// <summary>
        /// Fetch an array of documents using a GROQ query
        /// </summary>
        /// <param name="query">GROQ query</param>
        /// <param name="excludeDrafts">set to false if unpublished documents should be included in the result</param>
        /// <returns>Tuple of HttpStatusCode and T's wrapped in a FetchResult</returns>
        public virtual async Task<(HttpStatusCode, FetchResult<T>)> GetDocuments<T>(string query, bool excludeDrafts = true)
        {
            var encodedQ = System.Net.WebUtility.UrlEncode(query);
            var message = await _httpClient.GetAsync($"query/{_dataset}?query={encodedQ}").ConfigureAwait(false);
            return await FetchResultToResult<FetchResult<T>, T>(message, excludeDrafts).ConfigureAwait(false);
        }

        
        private async Task<(HttpStatusCode, T)> FetchResultToResult<T, V>(HttpResponseMessage message, bool excludeDrafts)
                where T : FetchResult<V>
        {
            if (!message.IsSuccessStatusCode)
            {
                return (message.StatusCode, null);
            }
            var content = await message.Content.ReadAsStringAsync().ConfigureAwait(false);

            var result = JsonConvert.DeserializeObject<T>(content);
            result.Result = excludeDrafts ?
                                result.Result.Where(doc => !doc.IsDraftDocument()).ToArray() :
                                result.Result;

            return (message.StatusCode, result);
        }

        /// <summary>
        /// Fetch an arbitrary query result using a GROQ query. This can be used where the response is not necessarily 
        /// expected to be an array of documents. Typical examples include aggregate queries such as count() or queries for a single property or document.
        /// </summary>
        /// <param name="query">GROQ query</param>
        /// <returns>Tuple of HttpStatusCode and T's wrapped in a FetchResult</returns>
        public virtual async Task<(HttpStatusCode, QueryResult<T>)> Query<T>(string query)
        {
            var encodedQ = System.Net.WebUtility.UrlEncode(query);
            var message = await _httpClient.GetAsync($"query/{_dataset}?query={encodedQ}").ConfigureAwait(false);
            return await QueryResultToResult<QueryResult<T>, T>(message, false).ConfigureAwait(false);
        }

        private async Task<(HttpStatusCode, T)> QueryResultToResult<T, V>(HttpResponseMessage message, bool excludeDrafts)
        where T : QueryResult<V>
        {
            if (!message.IsSuccessStatusCode)
            {
                return (message.StatusCode, null);
            }
            var content = await message.Content.ReadAsStringAsync().ConfigureAwait(false);

            var result = JsonConvert.DeserializeObject<T>(content);

            return (message.StatusCode, result);
        }


        /// <summary>
        /// Change one or more document using the given Mutations
        /// </summary>
        /// <param name="mutations">Mutations object containing mutations</param>
        /// <param name="returnIds">If true, the id's of modified documents are returned</param>
        /// <param name="returnDocuments">If true, the entire content of changed documents is returned</param>
        /// <param name="visibility">If "sync" the request will not return until the requested changes are visible to subsequent queries, if "async" the request will return immediately when the changes have been committed. For maximum performance, use "async" always, except when you need your next query to see the changes you made. "deferred" is used in cases where you are adding or mutating a large number of documents and don't need them to be immediately available.</param>
        public virtual async Task<(HttpStatusCode, MutationResult)> Mutate(
            Mutations mutations, bool returnIds = false, bool returnDocuments = false,
            Visibility visibility = Visibility.Sync)
        {
            var json = mutations.Serialize();
            var content = new StringContent(json);
            var url = $"mutate/{_dataset}?returnIds={returnIds.ToString().ToLower()}&returnDocuments={returnDocuments.ToString().ToLower()}&visibility={visibility.ToString().ToLower()}";
            var message = await _httpClient.PostAsync(url, content).ConfigureAwait(false);
            return await ResponseToResult<MutationResult>(message).ConfigureAwait(false);
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
