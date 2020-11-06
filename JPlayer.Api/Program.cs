using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace JPlayer.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }

    public class ApplicationVersion
    {
        public ApplicationVersion()
        {
            this.Name = typeof(Program).Assembly.GetName().Name;
            Version assemblyVersion = typeof(Program).Assembly.GetName().Version;
            if (assemblyVersion != null)
                this.Version = $"{assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Build}";
        }

        public string Name { get; }

        public string Version { get; }
    }
}