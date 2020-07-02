// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
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
            // Sono informazioni relative all'utente che è possibile aggiungere nel token.

            new IdentityResources.OpenId(),     // Id univoco (sub)
            new IdentityResources.Profile(),    // info aggiuntive sul profilo
        };

        /// <summary>
        /// API Resource
        /// </summary>
        public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
        {
            // Le resources è tutto ciò che è possibile proteggere con l'Identity Server.

            new ApiResource("service_one", "microservices one"),
            new ApiResource("api1", "microservices two"),
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
            new Client
            {
                ClientId = "postman",
                ClientName = "Postman client",
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },

                // ClientID & ClientSecret
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                AllowedScopes = { "api1" }
            },

            // Interactive ASP.NET UI client for Authorization Code Flow
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
                    new Claim("given_name", "Alice"),
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
                }
            }
        };
    }
}