using System;
using System.Net.Http;

namespace Olav.Sanity.Client
{
    public class SanityClient
    {
        private static readonly HttpClient HttpClient;

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
            
        }

        public object GetDocument(string id)
        {
            throw new NotImplementedException();
        }

        public object Fetch(string query, string parms)
        {
            throw new NotImplementedException();
        }
    }
}
