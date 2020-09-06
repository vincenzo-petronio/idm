using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Security.Claims;

namespace AuthorizationServer
{
    public static class Config
    {
        // Classe usata per la memorizzazione in-memory dei dati.

        // Uno scope è una risorsa che un client tenta di accedere.
        // In IdentityServer4 gli scope vengono modellati in due tipologie:
        // - IdentityResource per indicare gli scope relativi all'utente 
        // (es. email, last name, website, location, etc)
        // - ApiResource per indicare gli scope relativi all'API
        // (es. 'the calendar API' or 'read-only access to the calendar API')
        // Se nasce la necessità di accedere ad una API ma anche ad un dato dell'utente,
        // è possibile aggiungere la proprietà UserClaims nella definizione della 
        // ApiResource

        /// <summary>
        /// Identity Resources
        /// </summary>
        public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
        {
            // Sono informazioni (claims) relative all'utente che è possibile inviare al client 
            // per identificare un utente. Vengono inviate nell'ID Token attraverso il flusso
            // OpenID Connect.
            // Il client può richiederle attraverso AllowedScopes.

            new IdentityResources.OpenId(),     // Id univoco (sub)
            new IdentityResources.Profile(),    // info aggiuntive sul profilo

            // Oltre a quelli specificati dallo standard OpenId Connect,
            // se ne possono definire altri a piacere.
            //new IdentityResource("roles", "user role", new List<string>{ "role" }),
        };

        /// <summary>
        /// API Resources
        /// </summary>
        public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
        {
            // Le resources è tutto ciò che è possibile proteggere con l'Identity Server.
            // Generalmente sono le API.

            new ApiResource("service_one", "microservice one"),
            new ApiResource("service_two", "microservice two")
            {
                UserClaims = new List<string> { "role" }
            },
            new ApiResource("api_gtw", "API Gateway")
            {
                Scopes = { "user.basic" },

                // Essendo ApiResource una modellazione dello Scope per OAuth2,
                // l'aggiunta di un claim utente verrà inviata solo nell'access_token
                // e non nell'id_token!
                
                //UserClaims = new List<string> { "role" }
            },
        };

        /// <summary>
        /// API Scope
        /// </summary>
        public static IEnumerable<ApiScope> ApiScopes => new ApiScope[]
        {
            // Gli scope rappresentano ciò che è possibile fare con le API Resource,
            // esempio è possibile definire uno scope privilegiato per accedere 
            // in Read/Write, e uno scope base per accedere solo in Read.

            // Tra API Scope e Resource c'è una relazione 1:N, cioè tante resource
            // possono avere lo stesso scope.

            new ApiScope("user.super", "user with elevated privileges"),
            new ApiScope("user.basic", "user with basic privileges"),
        };

        /// <summary>
        /// API Clients
        /// </summary>
        public static IEnumerable<Client> Clients => new Client[]
        { 
            // Rappresenta i tipi di client che possono accedere alle risorse protette,
            // cioè i consumer possono accedere alle resource solo se sono "client" 
            // di questo tipo.
            // Un client può essere un browser, una application nativa, una macchina, 
            // un device, etc...

            // M2M machine to machine client for Client Code Flow
            // Usato nella comunicazione server-server, senza intervento umano.
            // Il client scambia ClientId e Secret per un AccessToken.
            new Client
            {
                ClientId = "m2m-client", // unique ID
                ClientName = "M2M client",
                ClientSecrets =
                {
                    new Secret("thisissosecret".Sha256())
                },

                // ClientID & ClientSecret
                AllowedGrantTypes = GrantTypes.ClientCredentials,

                AllowedScopes = { "user.basic" },
            },


            // Request da POSTMAN
            // Usato nella comunicazione da client sicuri.
            // Il client scambia Username e Password dell'user per un AccessToken.
            // I dati dell'user sono esposti al client, infatti non è un flow generalmente usato.
            new Client
            {
                ClientId = "postman-client",
                ClientName = "Postman client",
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },

                // ClientID & ClientSecret
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                AllowedScopes = { "user.basic" }
            },


            // Interactive ASP.NET UI client for Authorization Code Flow.
            // Usato nella comunicazione da web-app.
            // - L'user accede al client MVC;
            // - viene reindirizzato sulla login page dall'Identity Server;
            // - inserisce username/password;
            // - viene riportato indietro sul client MVC con un Code (esposto all'user!);
            // - il client MVC scambia il code con un AccessToken (non esposto all'user!);
            new Client
            {
                ClientId = "mvc-client",
                ClientName = "MVC client",
                ClientSecrets = { new Secret("thisissostrongersecret".Sha512()) },

                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,

                // where to redirect to after login
                RedirectUris =
                {
                    "http://localhost:15000/signin-oidc",
                    "http://host.docker.internal:15000/signin-oidc",
                },

                // where to redirect to after logout
                PostLogoutRedirectUris = { "http://localhost:15000/signout-callback-oidc" },

                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,

                    "user.basic",
                },

                AllowOfflineAccess = true,

                RequireConsent = true,

                AlwaysSendClientClaims = true,
                AlwaysIncludeUserClaimsInIdToken = true,
            },


            // SPA o Web/Mobile App for Authorization Code Flow with PKCE (Hybrid ???)
            // E' una combinazione tra Authorization Code e Implicit Flow.
            new Client
            {
                ClientId = "spa-client",
                ClientName = "SPA client",
                ClientSecrets = { new Secret("thisissostrongersecret".Sha512()) },

                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true, // il client genera una stringa random, l'hash 256 rappresenta il code_challenge

                RedirectUris =
                {
                    "http://localhost:15002/signin-oidc",
                    "http://host.docker.internal:15002/signin-oidc"
                },
                PostLogoutRedirectUris = { "http://localhost:15002/signout-callback-oidc" },

                AllowOfflineAccess = true,

                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    //"user.super"
                },

                Enabled = true,
            }
        };

        /// <summary>
        /// User di test in-memory
        /// </summary>
        public static List<TestUser> TestUsers => new List<TestUser>
        {
            new TestUser
            {
                SubjectId = "001",
                Username = "alice",
                Password = "alice",
                Claims = new List<Claim>
                {
                    // Un claim è una coppia chiave-valore, e rappresenta quello
                    // che il soggetto è, non quello che il soggetto può fare.
                    new Claim(ClaimTypes.GivenName, "Alice"),
                    new Claim(ClaimTypes.Role, "guest"),
                    new Claim(ClaimTypes.Email, "alice@email.com"),
                },
            },

            new TestUser
            {
                SubjectId = "002",
                Username = "admin",
                Password = "admin",
                Claims = new List<Claim>
                {
                    new Claim(ClaimTypes.GivenName, "Administrator"),
                    new Claim(ClaimTypes.Role, "admin"),
                    new Claim(ClaimTypes.Email, "admin@email.it"),
                }
            }
        };
    }
}