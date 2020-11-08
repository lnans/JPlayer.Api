using System;
using System.IO;
using System.Text.Json.Serialization;
using JPlayer.Business.Services;
using JPlayer.Data.Dao;
using JPlayer.Lib.Mapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace JPlayer.Api.AppStart
{
    // ReSharper disable once UnusedMember.Global
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        /// <summary>
        ///     This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            string connString = this._configuration.GetConnectionString("sqlite");
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite(connString);
                options.EnableSensitiveDataLogging();
                options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
            });
            services.AddTransient<UserService>();
            services.AddTransient<ProfileService>();
            services.AddTransient<FunctionService>();
            services.AddTransient<ObjectMapper>();
            services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.AddCustomSwaggerGen();
        }

        /// <summary>
        ///     This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "JPlayer.Api"));
            app.EnsureDbCreated();
        }
    }

    internal static class StartupHelper
    {
        public static void EnsureDbCreated(this IApplicationBuilder app)
        {
            using IServiceScope serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            ApplicationDbContext context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();
        }

        public static void AddCustomSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo {Title = "JPlayer.Api", Version = "v1"});
                options.DescribeAllParametersInCamelCase();
                string[] docs = Directory.GetFiles(AppContext.BaseDirectory, "JPlayer.*.xml", SearchOption.TopDirectoryOnly);
                foreach (string xmlPath in docs)
                    options.IncludeXmlComments(xmlPath);
            });
        }
    }
}