using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.HttpSys;

namespace DNT.IDP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                /*
                .UseHttpSys(options => // Just for local tests without IIS, Or self-hosted scenarios on Windows ...
                {
                    options.Authentication.Schemes =
                        AuthenticationSchemes.Negotiate | AuthenticationSchemes.NTLM;
                    options.Authentication.AllowAnonymous = true;
                    // you should run 
                    // netsh http add sslcert ipport=0.0.0.0:6001 certhash=<thumbprint of certificate> appid={new guid here} certstorename=Root
                })*/
                .UseStartup<Startup>();
    }
}
