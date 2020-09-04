using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcClient.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MvcClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Il client apre automaticamente l'Index, ma essendoci 
            // l'Authorize a livello globale, prova ad interrogare 
            // l'Identity Server
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }

        // Interroga l'API GTW passando l'access_token.
        public async Task<IActionResult> GetIdmClaims()
        {
            // Richiesta dell'access_token (specifico di OAuth2)
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            // Richiesta dell'id_token (specifico di OpenID)
            //var idToken = await HttpContext.GetTokenAsync("id_token");

            // Esattamente come il client CLI, si passa l'Authorization nell'header e si
            // chiama l'API con la risorsa desiderata.
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var content = await client.GetStringAsync("http://host.docker.internal:16000/idm/claims");

            // mostriamo nella pagina web quello che nel client CLI veniva mostrato sulla console.
            ViewBag.Json = JArray.Parse(content).ToString();
            return View("Claims");
        }

        // Interroga l'API GTW che a sua volta interroga il µService1, passando l'access_token.
        public async Task<IActionResult> GetRandomNumberFromService()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var content = await client.GetStringAsync("http://host.docker.internal:16000/services/numbers/random");

            ViewBag.Json = content;
            return View("Claims");
        }

        // Interroga l'API GTW che a sua volta interroga il µService2, passando l'access_token.
        public async Task<IActionResult> GetStringHelloFromService()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var content = await client.GetStringAsync("http://host.docker.internal:16000/services/strings/hello");

            ViewBag.Json = content;
            return View("Claims");
        }

        // Interroga direttamente il µService2 passando l'access_token.
        public async Task<IActionResult> GetStringVersionFromService()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            //{
            //    Address = "http://host.docker.internal:5000",
            //    Policy = { RequireHttps = false }
            //});

            //var userInfo = await client.GetUserInfoAsync(new UserInfoRequest
            //{
            //    Address = disco.UserInfoEndpoint,
            //    Token = accessToken
            //});

            //var email = userInfo.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email));

            //foreach (var c in userInfo.Claims)
            //{
            //    Console.WriteLine("###### HomeCtrl ######## " + c.Value);
            //}
            //Console.WriteLine(disco.UserInfoEndpoint);
            //Console.WriteLine(email);

            var content = await client.GetStringAsync("http://host.docker.internal:15200/strings/version");

            ViewBag.Json = content;
            return View("Claims");
        }
    }
}
