using System;
using JPlayer.Api.AppStart;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace JPlayer.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Logger logger = NLogBuilder.ConfigureNLog("log.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("Application start");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                LogManager.Flush();
                LogManager.Shutdown();
            }
        }

        private static Action<ILoggingBuilder> LogConfig()
        {
            return logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(LogLevel.Trace);
            };
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .ConfigureLogging(LogConfig())
                .UseNLog();
    }
}