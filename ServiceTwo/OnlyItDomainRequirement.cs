using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ServiceTwo
{
    public class OnlyItDomainRequirement : IAuthorizationRequirement
    {
        public OnlyItDomainRequirement()
        {
        }
    }

    public class OnlyItDomainAuthorizationHandler : AuthorizationHandler<OnlyItDomainRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OnlyItDomainRequirement requirement)
        {
            foreach (var c in context.User.Claims)
            {
                Console.WriteLine("###### Requirement ######## " + c.Value);
            }

            if (!context.User.HasClaim(x => x.Type == ClaimTypes.Email))
            {
                return Task.CompletedTask;
            }

            var emailAddress = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
            if (emailAddress.Split('@')[1].Equals("email.it"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
