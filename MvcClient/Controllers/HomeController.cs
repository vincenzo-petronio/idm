﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcClient.Models;
using Newtonsoft.Json.Linq;

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

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }

        public async Task<IActionResult> GetIdmClaims()
        {
            // Richiesta dell'AccessToken
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            // Esattamente come il client CLI, si passa l'Authorization nell'header e si
            // chiama l'API con la risorsa desiderata.
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var content = await client.GetStringAsync("https://localhost:6001/idm/claims");

            // mostriamo nella pagina web quello che nel client CLI veniva mostrato sulla console.
            ViewBag.Json = JArray.Parse(content).ToString();
            return View("Claims");
        }
    }
}
