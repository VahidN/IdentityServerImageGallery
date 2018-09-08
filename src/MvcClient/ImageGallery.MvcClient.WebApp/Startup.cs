using System.IdentityModel.Tokens.Jwt;
using IdentityModel;
using ImageGallery.MvcClient.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ImageGallery.MvcClient.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // register an IHttpContextAccessor so we can access the current
            // HttpContext in services by injecting it
            services.AddHttpContextAccessor();

            // register an IImageGalleryHttpClient
            services.AddHttpClient<IImageGalleryHttpClient, ImageGalleryHttpClient>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            }).AddCookie("Cookies", options =>
              {
                  options.AccessDeniedPath = "/Authorization/AccessDenied";
              })
              .AddOpenIdConnect("oidc", options =>
              {
                  options.SignInScheme = "Cookies";
                  options.Authority = Configuration["IDPBaseAddress"];
                  options.ClientId = "imagegalleryclient";
                  options.ResponseType = "code id_token";
                  //options.CallbackPath = new PathString("...")
                  //options.SignedOutCallbackPath = new PathString("...")
                  options.Scope.Add("openid");
                  options.Scope.Add("profile");
                  options.Scope.Add("address");
                  options.Scope.Add("roles");
                  options.Scope.Add("imagegalleryapi");

                  options.SaveTokens = true;
                  options.ClientSecret = "secret";
                  options.GetClaimsFromUserInfoEndpoint = true;

                  options.ClaimActions.Remove("amr");
                  options.ClaimActions.DeleteClaim("sid");
                  options.ClaimActions.DeleteClaim("idp");
                  // options.ClaimActions.DeleteClaim("address");

                  options.ClaimActions.MapUniqueJsonKey(claimType: "role", jsonKey: "role");

                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      NameClaimType = JwtClaimTypes.GivenName,
                      RoleClaimType = JwtClaimTypes.Role,
                  };
              });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Shared/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Gallery}/{action=Index}/{id?}");
            });
        }
    }
}
