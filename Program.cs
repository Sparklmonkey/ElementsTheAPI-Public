using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace ElementsTheAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var host = CreateWebHostBuilder(args);
            host.ConfigureKestrel(serverOptions =>
            {
                serverOptions.AllowSynchronousIO = true;
            });


            host.Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
