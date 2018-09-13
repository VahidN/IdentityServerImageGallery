using IdentityServer4.Services;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Extensions;
using System.Security.Claims;

namespace DNT.IDP.Services
{
    public class CustomUserProfileService : IProfileService
    {
        private readonly IUsersService _usersService;

        public CustomUserProfileService(IUsersService usersService)
        {
            _usersService = usersService;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            var claimsForUser = await _usersService.GetUserClaimsBySubjectIdAsync(subjectId);
            context.IssuedClaims = claimsForUser.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            context.IsActive = await _usersService.IsUserActiveAsync(subjectId);
        }
    }
}