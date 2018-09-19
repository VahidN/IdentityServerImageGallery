using DNT.IDP.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DNT.IDP
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddCustomUserStore(this IIdentityServerBuilder builder)
        {
            // builder.Services.AddScoped<IUsersService, UsersService>();
            builder.AddProfileService<CustomUserProfileService>();
            return builder;
        }
    }
}