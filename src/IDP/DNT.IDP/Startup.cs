using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using DNT.IDP.DataLayer.Context;
using DNT.IDP.Models;
using DNT.IDP.Services;
using DNT.IDP.Settings;
using DNT.IDP.Utils;
using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DNT.IDP
{
    public class Startup
    {
        public const string TwoFactorAuthenticationScheme = "idsrv.2FA";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, ApplicationDbContext>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IUserClaimsService, UserClaimsService>();
            services.AddScoped<IConfigSeedDataService, ConfigSeedDataService>();

            services.AddSingleton<IRandomNumberProvider, RandomNumberProvider>();
            services.AddScoped<ITwoFactorAuthenticationService, TwoFactorAuthenticationService>();

            services.Configure<OperationalStoreOptions>(options =>
                Configuration.GetSection("OperationalStoreOptions").Bind(options));


            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")
                        .Replace("|DataDirectory|",
                            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "app_data")),
                    serverDbContextOptionsBuilder =>
                    {
                        var minutes = (int) TimeSpan.FromMinutes(3).TotalSeconds;
                        serverDbContextOptionsBuilder.CommandTimeout(minutes);
                        serverDbContextOptionsBuilder.EnableRetryOnFailure();
                    });
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddIdentityServer()
                .AddSigningCredential(
                    //loadCertificateFromStore()
                    loadCertificateFromFile()
                )
                .AddCustomUserStore()
                .AddConfigurationStore()
                .AddOperationalStore();

            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows Account";
                iis.AutomaticAuthentication = false;
            });

            services.AddAuthentication()
                .AddCookie(authenticationScheme: TwoFactorAuthenticationScheme)
                .AddGoogle(authenticationScheme: "Google", configureOptions: options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.ClientId = Configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            initializeDb(app);
            seedDb(app);

            app.UseHttpsRedirection();

            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }

        private static void initializeDb(IApplicationBuilder app)
        {
            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetService<ApplicationDbContext>())
                {
                    context.Database.Migrate();
                }
            }
        }

        private static void seedDb(IApplicationBuilder app)
        {
            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var configSeedDataService = scope.ServiceProvider.GetService<IConfigSeedDataService>();
                configSeedDataService.EnsureSeedDataForContext(
                    Config.GetClients(),
                    Config.GetApiResources(),
                    Config.GetIdentityResources()
                );
            }
        }

        private X509Certificate2 loadCertificateFromFile()
        {
            // NOTE:
            // You should check out the identity of your application pool and make sure
            // that the `Load user profile` option is turned on, otherwise the crypto susbsystem won't work.
            var certificate = new X509Certificate2(
                fileName: Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "app_data", 
                    Configuration["X509Certificate:FileName"]),
                password: Configuration["X509Certificate:Password"],
                keyStorageFlags: X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet |
                                 X509KeyStorageFlags.Exportable);
            return certificate;
        }

        private X509Certificate2 loadCertificateFromStore()
        {
            var thumbPrint = Configuration["CertificateThumbPrint"];
            using (var store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly);
                var certCollection = store.Certificates.Find(
                    X509FindType.FindByThumbprint,
                    thumbPrint,
                    validOnly: true);
                if (certCollection.Count == 0)
                {
                    throw new Exception("The specified certificate wasn't found.");
                }

                return certCollection[0];
            }
        }
    }
}