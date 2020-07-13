using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace AuthorizationServer
{
    public static class Config
    {
        // Classe usata per la memorizzazione in-memory dei dati.

        /// <summary>
        /// Identity Resources
        /// </summary>
        public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
        {
            // Sono informazioni (claims) relative all'utente che è possibile inviare al client 
            // per identificare un utente. Vengono inviate nell'ID Token attraverso il flusso
            // OpenID Connect.

            new IdentityResources.OpenId(),     // Id univoco (sub)
            new IdentityResources.Profile(),    // info aggiuntive sul profilo
        };

        /// <summary>
        /// API Resource
        /// </summary>
        public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
        {
            // Le resources è tutto ciò che è possibile proteggere con l'Identity Server.
            // Generalmente sono le API.

            new ApiResource("service_one", "microservice one"),
            new ApiResource("service_two", "microservice two"),
        };

        /// <summary>
        /// API Scope
        /// </summary>
        public static IEnumerable<ApiScope> ApiScopes => new ApiScope[]
        {
            // Gli scope rappresentano ciò che è possibile fare con le API,
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

                // where to redirect to after login
                RedirectUris = { "https://localhost:5002/signin-oidc" },

                // where to redirect to after logout
                PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },

                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "user.basic"
                }
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

                RedirectUris = { "https://localhost:5003/signin-oidc" },
                PostLogoutRedirectUris = { "https://localhost:5003/signout-callback-oidc" },

                AllowOfflineAccess = true,

                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "user.super"
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
                    new Claim("given_name", "Alice"),
                    new Claim("role", "guest"),
                }
            },

            new TestUser
            {
                SubjectId = "002",
                Username = "admin",
                Password = "admin",
                Claims = new List<Claim>
                {
                    new Claim("given_name", "Administrator"),
                    new Claim("role", "admin"),
                }
            }
        };
    }
}