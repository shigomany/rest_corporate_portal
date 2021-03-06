using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using restcorporate_portal.Data.Initialize;
using restcorporate_portal.Models;

namespace restcorporate_portal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<corporateContext>();
                    SampleFiles.Initialize(context);
                    SampleDepartment.Initialize(context);
                    SampleSpecialities.Initialize(context);
                    SampleStatuses.Initialize(context);
                    SamplePriority.Initialize(context);
                    SampleDifficulties.Initialize(context);
                    SampleWorkers.Initialize(context);
                    SampleTasks.Initialize(context);
                    SampleProducts.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                webBuilder.UseKestrel();
                webBuilder.UseUrls("http://192.168.1.6:5000", "http://localhost:5000");
                    //webBuilder.UseHttpSys();
                    webBuilder.UseStartup<Startup>();
                });
    }
}
