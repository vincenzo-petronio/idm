//using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BlazorClient.Data
{
    public class ClaimsService
    {
        private readonly TokenProvider tokenProvider;
        private HttpClient httpClient { get; }

        public ClaimsService(IHttpClientFactory clientFactory, TokenProvider tokenProvider)
        {
            this.tokenProvider = tokenProvider;
            httpClient = clientFactory.CreateClient();
        }

        public async Task<string> GetClaims()
        {
            // Richiesta dell'AccessToken
            var accessToken = tokenProvider.AccessToken;

            // Si passa l'Authorization nell'header e si chiama l'API con la risorsa desiderata.
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var content = await client.GetStringAsync("https://localhost:6200/claims");

            // mostriamo nella pagina web quello che nel client CLI veniva mostrato sulla console.
            return JArray.Parse(content).ToString();
        }
    }
}
