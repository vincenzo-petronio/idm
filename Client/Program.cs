using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Hello Console!");

            // DISCO - Discovery Endpoint
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                Console.ReadKey(true);
                return;
            }

            // Richiesta dell'AccessToken passando i parametri richiesti, usando il client M2M
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "m2m-client",
                ClientSecret = "thisissosecret",
                Scope = "user.basic",
            });
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                Console.ReadKey(true);
                return;
            }
            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");


            // Richiesta dell'API usando l'AccessToken come Authorization nell'header
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);
            var response = await apiClient.GetAsync("https://localhost:6001/idm/claims");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }

            Console.ReadKey(true);
        }
    }
}
