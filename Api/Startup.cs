using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;

            services.AddControllers();

            services.AddAuthentication("Bearer")
                    .AddJwtBearer("Bearer", options =>
                    {
                        options.Authority = "http://host.docker.internal:5000";
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,

                            ValidateAudience = true,
                            // Corrisponde al valore in API Resources dell'Identity Server.
                            ValidAudiences = new List<string> { "api_gtw" },
                        };

                        options.Events = new JwtBearerEvents()
                        {
                            OnAuthenticationFailed = ctx =>
                            {
                                Console.WriteLine(ctx.Exception);
                                return Task.CompletedTask;
                            },
                            OnForbidden = ctx =>
                            {
                                Console.WriteLine(ctx.Response.StatusCode);
                                return Task.CompletedTask;
                            },
                        };
                    });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    //policy.RequireClaim("scope", "user.basic");
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello Api!");
                //});
                endpoints.MapControllers()
                    // policy globale valida per tutte le API
                    .RequireAuthorization("ApiScope")
                    ;
            });
        }
    }
}
