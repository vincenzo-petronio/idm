using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        [HttpGet]
        [Route("numbers/random")] // services/numbers/random
        public async Task<string> GetRandomNumber()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var apiClient = new HttpClient();
            apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // microservice 1
            var response = await apiClient.GetStringAsync("http://host.docker.internal:15100/numbers/random");
            return response;
        }

        [HttpGet]
        [Route("strings/hello")] // services/strings/hello
        public async Task<string> GetHello()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var apiClient = new HttpClient();
            apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // microservice 2
            var response = await apiClient.GetStringAsync("http://host.docker.internal:15200/strings/hello");
            return response;
        }
    }
}
