using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SyllabusZip.Common.Data;
using SyllabusZip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SyllabusZip.Factory
{
    public class CustomClaimsFactory : UserClaimsPrincipalFactory <UserModel>
    {
        public CustomClaimsFactory(UserManager<UserModel> userManager, IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(UserModel user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("firstname", user.FirstName));
            identity.AddClaim(new Claim("lastname", user.LastName));

            return identity;
        }
    }
}
